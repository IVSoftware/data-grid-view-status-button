You mention that you set the `ImageMode` to `Center` and you may want to use `Stretch` instead.

[![screenshot][1]][1]

I reproduced your post where two embedded 48 x 48 images are part of the class that represents a row in the bound data set and are coerced into a 32 x 32 display area:

    class Employee : INotifyPropertyChanged
    {
        public Employee()
        {
            ensureImages();
            Image = _imageInactive;
        }

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

***
**Main Form**

The main form formats the `DataGridView` and does a mock for where the database would read the data (since the database isn't part of the question).

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
            // Use Stretch here
            ((DataGridViewImageColumn)column)
            .ImageLayout = DataGridViewImageCellLayout.Stretch;
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


  [1]: https://i.stack.imgur.com/Fh955.png