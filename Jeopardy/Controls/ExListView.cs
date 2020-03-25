using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Jeopardy.Controls
{
    public class ExListView : ListView
    {
        public static readonly BindableProperty ItemClickProperty =
            BindableProperty.Create(nameof(ItemClick), typeof(ICommand), typeof(ExListView), null, BindingMode.OneWay);

        public ICommand ItemClick
        {
            get { return (ICommand)GetValue(ItemClickProperty); }
            set { SetValue(ItemClickProperty, value); }
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (this.Parent == null)
            {
                this.ItemSelected -= OnItemSelected;
            }
            else
            {
                this.ItemSelected += OnItemSelected;
            }
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // call command 
            ItemClick?.Execute(e.SelectedItem);

            // deselect item
            this.SelectedItem = null;
        }
    }
}
