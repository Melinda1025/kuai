using HandyControl.Interactivity;
using SDBS3000.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SDBS3000.Behaviors
{
    public class AutoLoadDataGridBehavior : Behavior<DataGrid>
    {
        public ICommand LoadMoreCommand
        {
            get { return (ICommand)GetValue(LoadMoreCommandProperty); }
            set { SetValue(LoadMoreCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoadMoreCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadMoreCommandProperty =
            DependencyProperty.Register("LoadMoreCommand", typeof(ICommand), typeof(AutoLoadDataGridBehavior), new PropertyMetadata(null));


        private ScrollViewer _scrollViewer;
        private double updateThreshold = 100;
        protected override void OnAttached()
        {            
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;                      
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            _scrollViewer = AssociatedObject.FindFirst<ScrollViewer>();
            if(_scrollViewer != null)
            {
                updateThreshold = AssociatedObject.RowHeight * 5;
                _scrollViewer.ScrollChanged += OnScrollChanged;
            }
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if((_scrollViewer.ScrollableHeight - _scrollViewer.VerticalOffset) < updateThreshold)
            {
                if(LoadMoreCommand != null && LoadMoreCommand.CanExecute(null))
                {
                    LoadMoreCommand.Execute(null);
                }
            }
        }

        protected override void OnDetaching() 
        {
            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged -= OnScrollChanged;
            }
            base.OnDetaching();
        }
    }
}
