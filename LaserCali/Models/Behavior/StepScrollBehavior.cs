using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace LaserCali.Models.Behavior
{
    public class StepScrollBehavior : Behavior<ScrollBar>
    {
        protected override void OnAttached()
        {
            AssociatedObject.ValueChanged += AssociatedObject_ValueChanged;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.ValueChanged -= AssociatedObject_ValueChanged;
            base.OnDetaching();
        }
        private void AssociatedObject_ValueChanged(object sender,
                System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            var scrollBar = (ScrollBar)sender;
            var newvalue = Math.Round(e.NewValue, 0);
            if (newvalue > scrollBar.Maximum)
                newvalue = scrollBar.Maximum;
            // feel free to add code to test against the min, too.
            scrollBar.Value = newvalue;
        }
    }
}
