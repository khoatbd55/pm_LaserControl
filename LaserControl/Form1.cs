using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using LaserControl.Api;
using LaserControl.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaserControl
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        ALSRCAL_PORT _slot = 0;
        IntPtr _hBox = IntPtr.Zero;
        public Form1()
        {
            this.Load += Form1_Load;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboBoardInfoType.Items.Clear();
            cboBoardInfoType.Items.Add(BRDINFOTYPE.BRDINFOTYPE_CARDINFO.ToString());
            cboBoardInfoType.Items.Add(BRDINFOTYPE.BRDINFOTYPE_SENSOR1INFO.ToString());
            cboBoardInfoType.Items.Add(BRDINFOTYPE.BRDINFOTYPE_SENSOR2INFO.ToString());
            cboBoardInfoType.Items.Add(BRDINFOTYPE.BRDINFOTYPE_SENSOR3INFO.ToString());
            cboBoardInfoType.Items.Add(BRDINFOTYPE.BRDINFOTYPE_SENSOR4INFO.ToString());
            cboBoardInfoType.Items.Add(BRDINFOTYPE.BRDINFOTYPE_SENSORSTYPE.ToString());
            cboBoardInfoType.SelectedIndex = 0;
        }

        private void btnFind2_Click(object sender, EventArgs e)
        {
            try
            {
                IntPtr hBoxs = IntPtr.Zero;
                ushort boxes = 1;
                ushort slotAtype=0;
                ushort slotBtype = 0;

                ALSRCAL_CODE code = (ALSRCAL_CODE)LaserApi.aLsrCalFind(ref hBoxs, ref slotAtype, ref slotBtype, ref boxes);
                WriteLog("call find 2 response code:" + code.ToString());
                if (slotAtype == (int)ALSRCAL_BOARD.ALSRCAL_PCAL_BOARD)
                {
                    _slot = ALSRCAL_PORT.ALSRCAL_PORT_A;
                    _hBox = hBoxs;
                    panelControl.Enabled = true;    
                    WriteLog($"slot A select {slotAtype} slot B: {slotBtype} box: {boxes} hBox:{_hBox.ToString()}");
                }
                else if (slotBtype == (int)ALSRCAL_BOARD.ALSRCAL_PCAL_BOARD)
                {
                    _slot = ALSRCAL_PORT.ALSRCAL_PORT_B;
                    _hBox = hBoxs;
                    panelControl.Enabled = true;
                    WriteLog($"slot A {slotAtype} slot B select: {slotBtype} box: {boxes} hBox:{_hBox.ToString()}");
                }
                else
                {
                    panelControl.Enabled = false;
                    WriteLog("can not find slot type.slotA:" + slotAtype + " slotB:" + slotBtype +" box:"+boxes);
                }
            }
            catch (Exception ex)
            {
                WriteLog("exception :" + ex.Message);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                ALSRCAL_CODE code = (ALSRCAL_CODE)LaserApi.aLsrCalPcalOpen(_hBox,(ushort) _slot);
                WriteLog($"open response code:{code.ToString()}");
            }
            catch (Exception ex)
            {
                WriteLog("exception :" + ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                ALSRCAL_CODE code = (ALSRCAL_CODE)LaserApi.aLsrCalPcalClose(_hBox, (ushort)_slot);
                WriteLog($"close response code:{code.ToString()}");
            }
            catch (Exception ex)
            {
                WriteLog("exception :" + ex.Message);
            }
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            try
            {
                ALSRCAL_CODE code = (ALSRCAL_CODE)LaserApi.aLsrCalPcalInit(_hBox, (ushort)_slot);
                WriteLog($"close response code:{code.ToString()}");
            }
            catch (Exception ex)
            {
                WriteLog("exception :" + ex.Message);
            }
        }

        private void btnReadPos_Click(object sender, EventArgs e)
        {
            try
            {
                LaserApi.POS pposLaser = new LaserApi.POS();
                LaserApi.POS pposEncoder = new LaserApi.POS();
                ulong ulSmplQty = 10;
                ulong pulSmplXfr = 0;
                ALSRCAL_CODE code = (ALSRCAL_CODE)LaserApi.aLsrCalPcalReadPos(_hBox, (ushort)_slot,ref pposLaser,ref pposEncoder, ulSmplQty,ref pulSmplXfr);
                WriteLog($"read pos response code:{code.ToString()}");
                WriteLog($"pposLaser:{JsonConvert.SerializeObject(pposLaser)}");
                WriteLog($"pposEncoder:{JsonConvert.SerializeObject(pposEncoder)}");
                WriteLog($"pulSmplXfr:{pulSmplXfr}");
            }
            catch (Exception ex)
            {
                WriteLog("exception :" + ex.Message);
            }
        }

        private void btnGetInfo2_Click(object sender, EventArgs e)
        {
            try
            {
                LaserApi.TU_LSRINFO_DEVICE_INFO pPortAInfo = new LaserApi.TU_LSRINFO_DEVICE_INFO();
                pPortAInfo.SerialNumber=new byte[16];
                LaserApi.TU_LSRINFO_DEVICE_INFO pPortBInfo = new LaserApi.TU_LSRINFO_DEVICE_INFO();
                pPortBInfo.SerialNumber = new byte[16];
                ALSRCAL_CODE code = (ALSRCAL_CODE)LaserApi.aLsrCalGetInfo(_hBox, (ushort)cboBoardInfoType.SelectedIndex,
                                                                                            ref pPortAInfo, ref pPortBInfo);
                WriteLog($"read info 2 response code:{code.ToString()}");
                WriteLog($"sensor A:{JsonConvert.SerializeObject(pPortAInfo)}");
                WriteLog($"sensor B:{JsonConvert.SerializeObject(pPortBInfo)}");
            }
            catch (Exception ex)
            {
                WriteLog("exception :" + ex.Message);
            }
            
        }

        private void btnReadVersion_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] hw = new byte[100];
                hw[0] = 99;
                byte[] fw=new byte[100];
                fw[0] = 99;
                ALSRCAL_CODE code = (ALSRCAL_CODE)LaserApi.aLsrCalGetVersions(_hBox, ref hw, ref fw);
                WriteLog($"read version response code:{code.ToString()}");
                hw[0] = (byte)':';
                fw[0] = (byte)':';
                WriteLog($"HW :{System.Text.Encoding.Default.GetString(hw)}");
                WriteLog($"FW :{System.Text.Encoding.Default.GetString(fw)}");
            }
            catch (Exception ex)
            {
                WriteLog("exception :" + ex.Message);
            }
        }

        private void btnReadSetting_Click(object sender, EventArgs e)
        {
            try
            {
                LaserApi.PCAL pcal = new LaserApi.PCAL();
                ALSRCAL_CODE code = (ALSRCAL_CODE)LaserApi.aLsrCalPcalGetSettings(_hBox, (ushort)_slot, ref pcal);
                WriteLog($"read version response code:{code.ToString()}");
                WriteLog($"setting info:{JsonConvert.SerializeObject(pcal)}");
            }
            catch (Exception ex)
            {
                WriteLog("exception :" + ex.Message);
            }
        }

        

        private void WriteLog(string msg)
        {
            listLog.Items.Add($"{DateTime.Now.ToLongTimeString()}:{msg}");
            while(listLog.Items.Count > 1000)
            {
                listLog.Items.RemoveAt(0);
            }
            listLog.SelectedIndex = listLog.Items.Count - 1;
        }

        
    }
}
