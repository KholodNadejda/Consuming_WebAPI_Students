using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;

namespace Consuming_WebAPI_Students
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
            List<string> status = new List<string>();
            status.Add("Finished");
            status.Add("Not Finished");
            cbxStatus.ItemsSource = status;

            client.BaseAddress = new Uri("https://localhost:7258");          
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.Loaded += MainWindow_Loaded;
        }

        async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try {
                HttpResponseMessage response = await client.GetAsync("/api/todoitems");
                response.EnsureSuccessStatusCode(); // throw an error code
                var students = await response.Content.ReadAsAsync<IEnumerable<Student>>();
                studentsListView.ItemsSource = students;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"MainWindow_Loaded {ex}");
            }
        }

        private async void btnGetStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("/api/todoitems" + txtID.Text);
                response.EnsureSuccessStatusCode(); //throw an error code
                var students = await response.Content.ReadAsAsync<Student>();
                studentDetailsPanel.Visibility = Visibility.Visible;
                studentDetailsPanel.DataContext = students;
            }
            catch (Exception)
            {
                MessageBox.Show("Student not found");
            }
        }

        private async void btnNewStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var student = new Student()
                {
                    User = txtStudentName.Text,
                    id = int.Parse(txtIDStudent.Text),
                    Status = cbxStatus.SelectedItem.ToString(),
                    Steps = int.Parse(txtSteps.Text)
                };
                var response = await client.PostAsJsonAsync("/api/todoitems", student);
                response.EnsureSuccessStatusCode(); //throw an error code           
                studentsListView.ItemsSource = await GetAllEstudantes();
                studentsListView.ScrollIntoView(studentsListView.ItemContainerGenerator.Items[studentsListView.Items.Count - 1]);
                MessageBox.Show("Student successfully added", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var student = new Student()
                {
                    User = txtStudentName.Text,
                    id = int.Parse(txtIDStudent.Text),
                    Status = cbxStatus.SelectedItem.ToString(),
                    Steps = int.Parse(txtSteps.Text)
                };
                var response = await client.PutAsJsonAsync("/api/todoitems", student);
                response.EnsureSuccessStatusCode(); //throw an error code
                studentsListView.ItemsSource = await GetAllEstudantes();
                studentsListView.ScrollIntoView(studentsListView.ItemContainerGenerator.Items[studentsListView.Items.Count - 1]);
                MessageBox.Show("Student successfully updated", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void btnDeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpResponseMessage response = await client.DeleteAsync("/api/todoitems" + txtID.Text);
                response.EnsureSuccessStatusCode(); 
                studentsListView.ItemsSource = await GetAllEstudantes();
                studentsListView.ScrollIntoView(studentsListView.ItemContainerGenerator.Items[studentsListView.Items.Count - 1]);
                MessageBox.Show("Student successfully deleted");
            }
            catch (Exception)
            {
                MessageBox.Show("Student successfully deleted");
            }
        }

        public async Task<IEnumerable<Student>> GetAllEstudantes()
        {
            HttpResponseMessage response = await client.GetAsync("/api/students");
            response.EnsureSuccessStatusCode(); //throws an error code
            var students = await response.Content.ReadAsAsync<IEnumerable<Student>>();
            return students;
        }
    }
}
