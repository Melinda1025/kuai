using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SDBS3000.Extensions
{
    public static class DependencyObjectExtension
    {
        public static T FindFirst<T>(this DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T target)
                {
                    return target;
                }
                else
                {
                    var innerTarget = FindFirst<T>(child);
                    if (innerTarget != null)
                    {
                        return innerTarget;
                    }
                }
            }
            return null;
        }
    }
}
