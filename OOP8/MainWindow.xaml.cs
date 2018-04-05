using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
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

namespace OOP8
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _currentUser;
        private UserContext context = new UserContext();
        public MainWindow()
        {
            InitializeComponent();

            Update();
            LoadMsg();
            UsersGrid.SelectedCellsChanged += UsersGrid_SelectedCellsChanged;
        }

        /*Методы работы с бд*/
        private void Update()
        {
            //var query = from c in context.Users
            //            select new { c.Id, c.Name, c.Password, c.Birthday };

            //var results = query.ToList();
            UsersGrid.ItemsSource = context.Users.ToList();
        }
        private void LoadMsg()
        {
            ChatList.ItemsSource = context.Messages.ToList();
        }
        private void Add(string name, string pass, DateTime birth)
        {
            context.Users.Add(new User() { Name = name, Password = pass, Birthday = birth });
            context.SaveChanges();
            LoadMsg();
        }

        /*Работа с Users Table*/
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Add(NickName.Text, Pass.Text, (DateTime)Birth.SelectedDate);
                NickName.Clear();
                Pass.Clear();
                Birth.Text = "";
            }
            catch
            {
                MessageBox.Show("Некорректная запись");
            }
            finally
            {
                Update();
            }
        }
        private void Del_Click(object sender, RoutedEventArgs e)
        {
            int cur_id = (UsersGrid.SelectedItem as User).Id;
            User cur = (from r in context.Users where r.Id == cur_id select r).SingleOrDefault();
            context.Users.Remove(cur);
            context.SaveChanges();
            Update();
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            Update();
        }
        private void UsersGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            context.SaveChanges();
        }

        /*Работа с Messages Table*/
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentUser = Login.Text;
                if (context.Users.Any(n => n.Name == _currentUser && n.Password == Password.Text))
                {
                    CurUsr.Content = _currentUser;
                }
                else
                {
                    throw new Exception("Некорректный логин или пароль");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Send_Message(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentUser == null)
                    throw new Exception("Войдите в учётную запись");
                context.Messages.Add(new Message() { Sender = _currentUser, Date = DateTime.Now, Text = MsgBox.Text });
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                LoadMsg();
            }
        }
    }

    /*Сущности и контекст*/
    class UserContext : DbContext
    {
        public UserContext()
            : base("SocialNetwork")
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public DateTime Birthday { get; set; }
        //public string Hobby { get; set; }
        //public BitmapImage Photo { get; set; }

        public User()
        {

        }
    }
    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}