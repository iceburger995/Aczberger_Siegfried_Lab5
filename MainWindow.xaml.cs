using AutoLotModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aczberger_Siegfried_Lab5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {

        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource inventoryViewSource;
        CollectionViewSource customerOrdersViewSource;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded()
        {
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));

            customerOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Customers.Load();

            ctx.Orders.Load();
            ctx.Inventories.Load();
            cmbCustomers.ItemsSource = ctx.Customers.Local;
            // cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";
            cmbInventory.ItemsSource = ctx.Inventories.Local;
            // cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";
            BindDataGrid();
        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Inventories on ord.CarID
                 equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarID,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            } 
            else if (action == ActionState.Edit) 
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                customerViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(customer);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
            }
        }

        private void btnSaveI_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory= null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    inventory = new Inventory()
                    {
                        Make = makeTextBox.Text.Trim(),
                        Color = colorTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Inventories.Add(inventory);
                    inventoryViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    inventory = (Inventory)customerDataGrid.SelectedItem;
                    inventory.Make = makeTextBox.Text.Trim();
                    inventory.Color = colorTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                inventoryViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                inventoryViewSource.View.MoveCurrentTo(inventory);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventories.Remove(inventory);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
            }
        }

        private void btnSaveO_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;
                    //instantiem Order entity
                    order = new Order()
                    {

                        CustId = customer.CustId,
                        CarID = inventory.CarId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarID = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(selectedOrder);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }
        private void btnNextI_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }
        private void btnNextO_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }
        private void btnPreviousI_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }
        private void btnPreviousO_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }
    }
}
