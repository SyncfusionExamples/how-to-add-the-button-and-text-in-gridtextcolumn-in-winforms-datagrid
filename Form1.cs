using DemoCommon.Grid;
using Syncfusion.Data;
using Syncfusion.WinForms.Core.Utils;
using Syncfusion.WinForms.DataGrid;
using Syncfusion.WinForms.DataGrid.Enums;
using Syncfusion.WinForms.DataGrid.Events;
using Syncfusion.WinForms.DataGrid.Helpers;
using Syncfusion.WinForms.DataGrid.Interactivity;
using Syncfusion.WinForms.DataGrid.Renderers;
using Syncfusion.WinForms.DataGrid.Styles;
using Syncfusion.WinForms.GridCommon.ScrollAxis;
using Syncfusion.WinForms.ListView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AddNewRow
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //To add custom renderer into SfDataGrid.
            this.sfDataGrid.CellRenderers.Add("TextButton", new GridTextButtonCellRenderer(this.sfDataGrid));
            sfDataGrid.AutoGenerateColumns = false;
            sfDataGrid.DataSource = new ViewModel().Orders;
            sfDataGrid.LiveDataUpdateMode = LiveDataUpdateMode.AllowDataShaping;

            GridTextColumn gridTextColumn1 = new GridTextColumn() { MappingName = "OrderID", HeaderText = "Order ID" };
            GridTextColumn gridTextColumn3 = new GridTextColumn() { MappingName = "CustomerName", HeaderText = "Customer Name" };
            GridTextColumn gridTextColumn4 = new GridTextColumn() { MappingName = "Country", HeaderText = "Country" };
            GridTextColumn gridTextColumn5 = new GridTextColumn() { MappingName = "ShipCity", HeaderText = "Ship City" };
            GridCheckBoxColumn checkBoxColumn = new GridCheckBoxColumn() { MappingName = "IsShipped", HeaderText = "Is Shipped" };

            //To add TextButtonColumn in grid
            this.sfDataGrid.Columns.Add(new GridTextButtonColumn() { MappingName = "CustomerID", Width = 140 });
           
           
            sfDataGrid.Columns.Add(gridTextColumn1);
            sfDataGrid.Columns.Add(gridTextColumn3);
            sfDataGrid.Columns.Add(gridTextColumn4);
            sfDataGrid.Columns.Add(gridTextColumn5);
            sfDataGrid.Columns.Add(checkBoxColumn);

        }

          
    }

    #region New CustomColumn
    public class GridTextButtonColumn : GridTextColumn
    {
        private CellButton cellButton;

        public CellButton CellButton
        {
            get { return cellButton; }
            set { cellButton = value; }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GridRatingColumn"/> class.
        /// </summary>
        public GridTextButtonColumn()
        {
            SetCellType("TextButton");
        }

    }

    #endregion


    #region CustomSelectioncontroller
    public class CustomSelectionController : RowSelectionController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomSelectionController"/> class.
        /// </summary>
        /// <param name="dataGrid">The DataGrid.</param>
        public CustomSelectionController(SfDataGrid dataGrid)
            : base(dataGrid)
        {
        }

        /// <summary>
        /// Process the key operations for SfDataGrid.
        /// </summary>
        /// <param name="e">that contains the event data.</param>
        public new void HandleKeyOperations(KeyEventArgs e)
        {
            base.HandleKeyOperations(e);
        }
    }

    #endregion


    #region Custom Column Cell renderer
    public class GridTextButtonCellRenderer : GridTextBoxCellRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridRatingCellRenderer"/> class.
        /// </summary>
        /// <param name="eclipse">The rating control.</param>
        /// <param name="dataGrid">The DataGrid.</param>
        public GridTextButtonCellRenderer(SfDataGrid dataGrid)
        {
            IsEditable = true;
            DataGrid = dataGrid;
        }

        /// <summary>
        /// Gets or Sets to specifies the datagrid.
        /// </summary>
        protected SfDataGrid DataGrid { get; set; }

        protected override void OnRender(Graphics paint, Rectangle cellRect, string cellValue, CellStyleInfo style, DataColumnBase column, RowColumnIndex rowColumnIndex)
        {
            base.OnRender(paint, cellRect, cellValue, style, column, rowColumnIndex);

            //To set the rectangle for button in the cell.
            var rect = new Rectangle(cellRect.Location.X + cellRect.Width - 22, cellRect.Location.Y, 20, cellRect.Height);

            (column.GridColumn as GridTextButtonColumn).CellButton = new CellButton();
            (column.GridColumn as GridTextButtonColumn).CellButton.Image = Image.FromFile(@"..\..\Images\icons.png");
            (column.GridColumn as GridTextButtonColumn).CellButton.TextImageRelation = TextImageRelation.ImageBeforeText;

            PropertyInfo highlightedItemProperty = (column.GridColumn as GridTextButtonColumn).CellButton.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Single(pi => pi.Name == "Bounds");
            highlightedItemProperty.SetValue((column.GridColumn as GridTextButtonColumn).CellButton, rect);

            //To draw the button in cell
            DrawButton(paint, cellRect, rect, "...", new ButtonCellStyleInfo(), column, rowColumnIndex);

        }

        public virtual void DrawButton(Graphics paint, Rectangle cellRect, Rectangle buttonRect, string cellValue, ButtonCellStyleInfo style, DataColumnBase column, Syncfusion.WinForms.GridCommon.ScrollAxis.RowColumnIndex rowColumnIndex, int buttonIndex = 0)
        {
            // No need to draw the button when its not visible on the cell bounds.
            if (cellRect.Width < 5)
                return;         

            var clipBound = paint.ClipBounds;
            var cellButton = (column.GridColumn as GridTextButtonColumn).CellButton;


            bool drawHovered = false;

            DrawBackground(paint, buttonRect, style, drawHovered,cellButton);

            if (cellRect.Contains(buttonRect))
                paint.SetClip(buttonRect);
            else if (cellRect.IntersectsWith(buttonRect))
            {
                Rectangle intersectRect = Rectangle.Intersect(cellRect, buttonRect);
                paint.SetClip(intersectRect);
            }

            if (cellButton.Image != null)
            {
                var imageSize = cellButton.Image.Size.IsEmpty ? cellButton.Image.Size : Size.Empty;
                Rectangle imageRectangle = buttonRect;
                DrawImage(paint, imageRectangle, cellButton.Image);
            }
               
            paint.SetClip(cellRect);
            DrawBorder(paint, buttonRect, style, drawHovered);
            paint.SetClip(clipBound);
        }

        private void DrawBorder(Graphics paint, Rectangle buttonRect, ButtonCellStyleInfo style, bool drawHovered)
        {
            if (style.Enabled)
            {
                if (style.BorderColor != null)
                    paint.DrawRectangle(style.BorderColor, Rectangle.Round(buttonRect));
            }
            else
            {
                if (style.DisabledBorderColor != null)
                    paint.DrawRectangle(style.DisabledBorderColor, Rectangle.Round(buttonRect));
            }
        }

       
        private void DrawBackground(Graphics paint, Rectangle buttonRect, ButtonCellStyleInfo style, bool drawHovered,CellButton cellButton)
        {
            Color color = style.BackColor;
            if (style.Enabled)
            {

                color = style.BackColor;
            }
            else
            {
                color = style.DisabledBackColor;
            }

            paint.FillRectangle(new SolidBrush(color), buttonRect);
        }

        protected internal virtual void DrawImage(Graphics graphics, Rectangle bounds, Image image)
        {
            graphics.DrawImage(image, Rectangle.Ceiling(bounds), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
        }

        protected override void OnMouseDown(DataColumnBase dataColumn, RowColumnIndex rowColumnIndex, MouseEventArgs e)
        {
            base.OnMouseDown(dataColumn, rowColumnIndex, e);
            var cellButton = (dataColumn.GridColumn as GridTextButtonColumn).CellButton;
            PropertyInfo highlightedItemProperty = (dataColumn.GridColumn as GridTextButtonColumn).CellButton.GetType().GetProperty("Bounds",BindingFlags.NonPublic|BindingFlags.Instance);//.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Single(pi => pi.Name == "Bounds");
            Rectangle rect =(Rectangle)highlightedItemProperty.GetValue((dataColumn.GridColumn as GridTextButtonColumn).CellButton);
          if(e.Location.X > rect.X && e.Location.X < (rect.X + rect.Width))
            {
                MessageBox.Show("Button Clicked");
            }
            
        }

        /// <summary>
        /// Occurs when the key is pressed while the cell has focus.
        /// </summary>
        /// <param name="dataColumn">The DataColumnBase of the cell.</param>
        /// <param name="rowColumnIndex">The row and column index of the cell.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(DataColumnBase dataColumn, RowColumnIndex rowColumnIndex, KeyEventArgs e)
        {
            var selectionController = this.DataGrid.SelectionController as CustomSelectionController;
            switch (e.KeyCode)
            {
                case Keys.Space:
                case Keys.Down:
                case Keys.Up:
                case Keys.Left:
                case Keys.Right:
                case Keys.Enter:
                case Keys.PageDown:
                case Keys.PageUp:
                case Keys.Tab:
                case Keys.Home:
                case Keys.End:
                    selectionController.HandleKeyOperations(e);
                    break;
            }

            base.OnKeyDown(dataColumn, rowColumnIndex, e);
        }
    }

    #endregion

    public class OrderInfo : INotifyPropertyChanged
    {
        decimal? orderID;
        string customerId;
        string country;
        string customerName;
        string shippingCity;
        bool isShipped;

        public OrderInfo()
        {

        }

        public decimal? OrderID
        {
            get { return orderID; }
            set { orderID = value; this.OnPropertyChanged("OrderID"); }
        }

        public string CustomerID
        {
            get { return customerId; }
            set { customerId = value; this.OnPropertyChanged("CustomerID"); }
        }

        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; this.OnPropertyChanged("CustomerName"); }
        }

        public string Country
        {
            get { return country; }
            set { country = value; this.OnPropertyChanged("Country"); }
        }

        public string ShipCity
        {
            get { return shippingCity; }
            set { shippingCity = value; this.OnPropertyChanged("ShipCity"); }
        }

        public bool IsShipped
        {
            get { return isShipped; }
            set { isShipped = value; this.OnPropertyChanged("IsShipped"); }
        }


        public OrderInfo(decimal? orderId, string customerName, string country, string customerId, string shipCity, bool isShipped)
        {
            this.OrderID = orderId;
            this.CustomerName = customerName;
            this.Country = country;
            this.CustomerID = customerId;
            this.ShipCity = shipCity;
            this.IsShipped = isShipped;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ViewModel
    {
        private ObservableCollection<OrderInfo> orders;
        public ObservableCollection<OrderInfo> Orders
        {
            get { return orders; }
            set { orders = value; }
        }

        public ViewModel()
        {
            orders = new ObservableCollection<OrderInfo>();
            orders.Add(new OrderInfo(1001, "Thomas Hardy", "Germany", "ALFKI", "Berlin", true));
            orders.Add(new OrderInfo(1002, "Laurence Lebihan", "Mexico", "ANATR", "Mexico D.F.", false));
            orders.Add(new OrderInfo(1003, "Antonio Moreno", "Mexico", "ANTON", "Mexico D.F.", true));
            orders.Add(new OrderInfo(1004, "Thomas Hardy", "UK", "AROUT", "London", true));
            orders.Add(new OrderInfo(1005, "Christina Berglund", "Sweden", "BERGS", "Lula", false));
            orders.Add(new OrderInfo(1006, "Thomas Hardy", "Germany", "ALFKI", "Berlin", true));
            orders.Add(new OrderInfo(1007, "Laurence Lebihan", "Mexico", "ANATR", "Mexico D.F.", false));
            orders.Add(new OrderInfo(1008, "Antonio Moreno", "Mexico", "ANTON", "Mexico D.F.", true));
            orders.Add(new OrderInfo(1009, "Thomas Hardy", "UK", "AROUT", "London", true));
            orders.Add(new OrderInfo(1000, "Christina Berglund", "Sweden", "BERGS", "Lula", false));
        }
    }

}
