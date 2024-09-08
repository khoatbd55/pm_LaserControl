using Basler.Pylon;
using KUtilities.TaskExtentions;
using LaserCali.Models.Camera;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaserCali.Services
{
    public class KCameraService
    {
        public delegate void OnImageHandle(object sender, CameraImage_EventArgs e);
        public delegate void OnConnectionHandle(object sender, CameraConnection_EventArg e);
        public event OnImageHandle OnImage;
        public event OnConnectionHandle OnConnection;



        // Frame rate at which an image is output for rendering.
        // Rendering very large images can cause a high load on the CPU.
        // Because of this, images are rendered at a rate of 10 frames per second.
        // You can adjust this value as required.
        private readonly double RENDERFPS = 10;

        CancellationTokenSource _backgroundCancelTokenSource = new CancellationTokenSource();
        KAsyncQueue<Exception> _stopQueue = new KAsyncQueue<Exception>();
        Task _taskStop;
        Task _taskCamera;
        object _lockStop = new object();
        Camera camera;
        // Invert pixels for an example of image processing.
        private bool invertPixels = false;
        // The pixel data converter is used for converting grabbed images to a pixel format that can be displayed.
        private PixelDataConverter converter = new PixelDataConverter();

        public Bitmap GetLatestFrame()
        {
            lock (monitor)
            {
                if (latestFrame != null)
                {
                    Bitmap returnedBitmap = latestFrame;
                    latestFrame = null;
                    return returnedBitmap;
                }
                return null;
            }
        }

        // Monitor object for managing concurrent thread access to latestFrame.
        private Object monitor = new Object();
        // Buffer for latest image.
        private Bitmap latestFrame = null;

        // Stopwatch to slow down rendering if the camera is grabbing faster than the monitor display rate.
        private System.Diagnostics.Stopwatch stopwatch;
        // Minimum duration between frames in stopwatch ticks.
        private int frameDurationTicks;
        long _step = 0;
        public KCameraService()
        {
            stopwatch = new System.Diagnostics.Stopwatch();
            // Calculate the number of stopwatch ticks for every frame at the given frame rate.
            double frametime = 1 / RENDERFPS;
            frameDurationTicks = (int)(System.Diagnostics.Stopwatch.Frequency * frametime);
        }

        public void Run()
        {
            _backgroundCancelTokenSource = new CancellationTokenSource();
            var c = _backgroundCancelTokenSource.Token;
            _stopQueue = new KAsyncQueue<Exception>();
            _taskStop = Task.Run(() => ProcessStopAllTask(c), c);
            _taskCamera = Task.Run(() => ProcessCamera(c), c);
        }

        private void InvertColors(IGrabResult result)
        {
            byte pixelSize = 255;
            byte[] data = (byte[])result.PixelData;
            for (long i = 0; i < data.Length; i++)
            {
                data[i] = (byte)((int)pixelSize - (int)data[i]);
            }
        }

        private async Task ProcessCamera(CancellationToken c)
        {

            while (!c.IsCancellationRequested)
            {
                try
                {
                    long step = Interlocked.Read(ref _step);
                    switch (step)
                    {
                        case 0:
                            {
                                camera = new Camera();
                                // Print the model name of the camera.
                                Console.WriteLine("Using camera {0}.", camera.CameraInfo[CameraInfoKey.ModelName]);

                                // Set the acquisition mode to free running continuous acquisition when the camera is opened.
                                camera.CameraOpened += Configuration.AcquireContinuous;

                                // Open the connection to the camera device.
                                camera.Open();
                                // The parameter MaxNumBuffer can be used to control the amount of buffers
                                // allocated for grabbing. The default value of this parameter is 10.
                                camera.Parameters[PLCameraInstance.MaxNumBuffer].SetValue(5);

                                // Start grabbing.
                                camera.StreamGrabber.Start();
                                //camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
                                //camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;
                                camera.ConnectionLost += OnConnectionLost;
                                if (OnConnection != null)
                                {
                                    OnConnection(this, new CameraConnection_EventArg() { IsConnected = true });
                                }
                                Interlocked.Exchange(ref _step, 1);
                            }
                            break;
                        case 1:
                            {
                                // Limit the number of frames passed to the image window to the display frame rate specified.
                                if (!stopwatch.IsRunning || stopwatch.ElapsedTicks >= frameDurationTicks)
                                {
                                    stopwatch.Restart();
                                    // Wait for an image and then retrieve it. A timeout of 5000 ms is used.
                                    IGrabResult grabResult = camera.StreamGrabber.RetrieveResult(5000, TimeoutHandling.ThrowException);
                                    using (grabResult)
                                    {
                                        // Image grabbed successfully?
                                        if (grabResult.GrabSucceeded)
                                        {
                                            // Example of image processing.
                                            if (grabResult.PixelTypeValue.BitDepth() == 8 && invertPixels)
                                            {
                                                InvertColors(grabResult);
                                            }
                                            // Access the image data.
                                            //Console.WriteLine("SizeX: {0}", grabResult.Width);
                                            //Console.WriteLine("SizeY: {0}", grabResult.Height);

                                            Bitmap bitmap = new Bitmap(grabResult.Width, grabResult.Height, PixelFormat.Format32bppRgb);
                                            // Lock the bits of the bitmap.
                                            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                                            converter.OutputPixelFormat = PixelType.BGRA8packed;
                                            // Place the pointer to the buffer of the bitmap.
                                            IntPtr ptrBmp = bmpData.Scan0;
                                            converter.Convert(ptrBmp, bmpData.Stride * bitmap.Height, grabResult);
                                            bitmap.UnlockBits(bmpData);
                                            lock (monitor)
                                            {
                                                if (latestFrame != null)
                                                {
                                                    latestFrame.Dispose();
                                                }
                                                latestFrame = bitmap;
                                            }
                                            if (OnImage != null)
                                            {
                                                OnImage(this, new CameraImage_EventArgs()
                                                {
                                                    Image = bitmap,
                                                });
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    await Task.Delay(1, c);
                                }

                            }
                            break;
                        case 2:
                            {
                                await Task.Delay(100, c);
                            }
                            break;
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        if (camera != null)
                        {
                            camera.StreamGrabber.Stop();
                            // Close the connection to the camera device.
                            camera.Close();
                        }

                    }
                    catch (Exception)
                    {

                    }
                    await Task.Delay(100, c);
                    Interlocked.Exchange(ref _step, 0);
                    if (OnConnection != null)
                    {
                        OnConnection(this, new CameraConnection_EventArg() { IsConnected = false });
                    }
                }

            }
        }

        private void OnConnectionLost(object sender, EventArgs e)
        {
            Interlocked.Exchange(ref _step, 0);
        }

        //private void OnImageGrabbed(object sender, ImageGrabbedEventArgs e)
        //{
        //    try
        //    {
        //        // Wait for an image and then retrieve it. A timeout of 5000 ms is used.
        //        IGrabResult grabResult = e.GrabResult;
        //        using (grabResult)
        //        {
        //            // Image grabbed successfully?
        //            if (grabResult.GrabSucceeded)
        //            {
        //                // Limit the number of frames passed to the image window to the display frame rate specified.
        //                if (!stopwatch.IsRunning || stopwatch.ElapsedTicks >= frameDurationTicks)
        //                {
        //                    stopwatch.Restart();
        //                    // Example of image processing.
        //                    if (grabResult.PixelTypeValue.BitDepth() == 8 && invertPixels)
        //                    {
        //                        InvertColors(grabResult);
        //                    }
        //                    // Access the image data.
        //                    Console.WriteLine("SizeX: {0}", grabResult.Width);
        //                    Console.WriteLine("SizeY: {0}", grabResult.Height);

        //                    Bitmap bitmap = new Bitmap(grabResult.Width, grabResult.Height, PixelFormat.Format32bppRgb);
        //                    // Lock the bits of the bitmap.
        //                    BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
        //                    converter.OutputPixelFormat = PixelType.BGRA8packed;
        //                    // Place the pointer to the buffer of the bitmap.
        //                    IntPtr ptrBmp = bmpData.Scan0;
        //                    converter.Convert(ptrBmp, bmpData.Stride * bitmap.Height, grabResult);
        //                    bitmap.UnlockBits(bmpData);
        //                    lock (monitor)
        //                    {
        //                        if (latestFrame != null)
        //                        {
        //                            latestFrame.Dispose();
        //                        }
        //                        latestFrame = bitmap;
        //                    }
        //                    if (OnImage != null)
        //                    {
        //                        OnImage(this, new CameraImage_EventArgs()
        //                        {
        //                            Image = bitmap
        //                        });
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                Console.WriteLine("Error: {0} {1}", grabResult.ErrorCode, grabResult.ErrorDescription);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }

        //}

        private async Task ProcessStopAllTask(CancellationToken c)
        {
            try
            {
                while (!c.IsCancellationRequested)
                {
                    var stop = await _stopQueue.TryDequeueAsync(c).ConfigureAwait(false);
                    if (stop.IsSuccess)
                    {
                        await CloseCoreAsync(stop.Item).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception)
            {
                //_logger.LogError("{time} {0} cache service - fail in stop task {1}", DateTime.Now, this._name, e.Message);
            }
        }

        public async Task CloseCoreAsync(Exception ex)
        {
            lock (_lockStop)
            {
                _backgroundCancelTokenSource?.Cancel();
            }
            await WaitForTask(_taskCamera).ConfigureAwait(false);
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }
            try
            {
                if (camera != null)
                {
                    camera.StreamGrabber.Stop();
                    // Close the connection to the camera device.
                    camera.Close();
                }
            }
            catch (Exception)
            {

            }
            //_logQueue.Clear();
        }

        private async Task WaitForTask(Task task)
        {
            try
            {
                if (task != null && !task.IsCompleted)
                    await task.ConfigureAwait(false);
            }
            catch (Exception)
            {

            }
        }

        private void OnClosing(Exception e)
        {
            lock (_lockStop)
            {
                if (!_backgroundCancelTokenSource.Token.IsCancellationRequested)
                {
                    _stopQueue.Enqueue(e);
                }
            }
        }

        public async Task StopAsync()
        {
            this.OnClosing(new Exception("disconnect by require"));
            await WaitForTask(_taskStop).ConfigureAwait(false);
        }

    }
}
