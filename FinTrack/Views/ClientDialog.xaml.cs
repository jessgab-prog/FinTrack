using System.Windows;
using FinTrack.Data;
using FinTrack.Models;

namespace FinTrack.Views;

public partial class ClientDialog : Window
{
    private readonly int? _editId;

    public ClientDialog(int? editId = null)
    {
        InitializeComponent();
        _editId = editId;

        if (editId.HasValue)
        {
            TxtTitle.Text = "Edit Client";
            using var db = DatabaseHelper.GetContext();
            var c = db.Clients.Find(editId.Value);
            if (c == null) return;
            CmbType.SelectedIndex = c.Type == "Client" ? 0 : 1;
            TxtName.Text = c.Name;
            TxtContact.Text = c.ContactNumber;
            TxtEmail.Text = c.Email;
            TxtAddress.Text = c.Address;
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtName.Text))
        { ShowError("Name is required."); return; }

        using var db = DatabaseHelper.GetContext();

        Client client = _editId.HasValue
            ? db.Clients.Find(_editId.Value)!
            : new Client();

        client.Type = (CmbType.SelectedItem as System.Windows.Controls.ComboBoxItem)!
                                .Content.ToString()!;
        client.Name = TxtName.Text.Trim();
        client.ContactNumber = TxtContact.Text.Trim();
        client.Email = TxtEmail.Text.Trim();
        client.Address = TxtAddress.Text.Trim();

        if (!_editId.HasValue) db.Clients.Add(client);
        db.SaveChanges();

        DialogResult = true;
        Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ShowError(string msg)
    {
        TxtError.Text = msg;
        TxtError.Visibility = Visibility.Visible;
    }
}