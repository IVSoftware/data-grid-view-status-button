using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace data_grid_view_status_button
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            dgvStatustableau.DataSource = Employees;
            dgvStatustableau.AllowUserToAddRows= false;

            #region F O R M A T    C O L U M N S
            Employees.Add(new Employee());
            DataGridViewColumn column;
            column = dgvStatustableau.Columns[nameof(Employee.FullName)];
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            column = dgvStatustableau.Columns[nameof(Employee.Image)];
            column.HeaderText= string.Empty;
            column.Width = 32;
            ((DataGridViewImageColumn)column).ImageLayout = DataGridViewImageCellLayout.Stretch;
            Employees.Clear();
            #endregion F O R M A T    C O L U M N S

            mockAddEmployees();
        }
        private BindingList<Employee> Employees { get; } = new BindingList<Employee>();

        private void mockAddEmployees()
        {
            Employees.Add(new Employee
            {
                FullName= "Lisa Smith",
                IsActive= true,
            });
            Employees.Add(new Employee
            {
                FullName= "Bob Jones",
                IsActive= false,
            });
            Employees.Add(new Employee
            {
                FullName= "Rene Montoya",
                IsActive= true,
            });
        }
    }

    class Employee : INotifyPropertyChanged
    {
        public string FullName { get; set; }

        public Image Image
        {
            get => _image;
            set
            {
                if (!Equals(_image, value))
                {
                    _image = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Image)));
                }
            }
        }
        Image _image = null;

        [Browsable(false)]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (!Equals(_isActive, value))
                {
                    _isActive = value;
                    Image = _isActive ? _imageActive : _imageInactive;
                }
            }
        }
        bool _isActive = false;

        public Employee()
        {
            ensureImages();
            Image = _imageInactive;
        }

        private void ensureImages()
        {
            if(_imageActive == null) 
            {
                var names = typeof(Employee).Assembly.GetManifestResourceNames();
                _imageActive = localImageFromResourceName(names.First(_=>_.Contains("active.png")));
                _imageInactive = localImageFromResourceName(names.First(_ => _.Contains("inactive.png")));

                Image localImageFromResourceName(string resource)
                {
                    using (var stream = GetType().Assembly.GetManifestResourceStream(resource)!)
                    {
                        return new Bitmap(stream);
                    }
                }
            }
        }

        private static Image _imageActive = null;
        private static Image _imageInactive = null;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
