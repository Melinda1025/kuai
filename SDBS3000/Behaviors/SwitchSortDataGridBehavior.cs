using HandyControl.Interactivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace SDBS3000.Behaviors
{
    public class SwitchSortDataGridBehavior : Behavior<DataGrid>
    {
        private ICollectionView _view;

        protected override void OnAttached()
        {
            base.OnAttached();
            _view = CollectionViewSource.GetDefaultView(AssociatedObject.ItemsSource);
            AssociatedObject.Sorting += AssociatedObject_Sorting;
        }

        private void AssociatedObject_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;
            _view.SortDescriptions.Clear();
            switch (e.Column.SortDirection)
            {
                case null:
                    e.Column.SortDirection = ListSortDirection.Ascending;
                    _view.SortDescriptions.Add(new(e.Column.SortMemberPath, ListSortDirection.Ascending));
                    break;
                case ListSortDirection.Ascending:
                    e.Column.SortDirection = ListSortDirection.Descending;
                    _view.SortDescriptions.Add(new(e.Column.SortMemberPath, ListSortDirection.Descending));
                    break;
                case ListSortDirection.Descending:
                    e.Column.SortDirection = null;
                    break;
            }
            _view.Refresh();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Sorting -= AssociatedObject_Sorting;
            _view = null;
        }


    }
}
