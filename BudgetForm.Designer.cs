using System;
using System.Windows.Forms;

namespace BudgetManagement
{
    public partial class BudgetForm : Form
    {
        private BudgetManager budgetManager;

        public BudgetForm()
        {
            InitializeComponent();
            budgetManager = new BudgetManager();
            UpdateTransactionsList();
            UpdateTotalBudget();

            typeComboBox.Items.Add("Доход");
            typeComboBox.Items.Add("Расход");
            typeComboBox.SelectedIndex = 0;

            addTransactionButton.Click += AddTransactionButton_Click;
            removeTransactionButton.Click += RemoveTransactionButton_Click;
            updateTransactionButton.Click += UpdateTransactionButton_Click;
        }

        private void UpdateTransactionsList()
        {
            transactionsListBox.Items.Clear();
            foreach (var t in budgetManager.Transactions)
            {
                string type = t.Type == TransactionType.Доход ? "+" : "-";
                transactionsListBox.Items.Add($"{t.Description} | {type} {t.Amount} руб. | {t.Date:dd.MM.yyyy}");
            }
        }

        private void UpdateTotalBudget()
        {
            totalBudgetLabel.Text = $"Общий бюджет: {budgetManager.TotalBudget} руб.";
            totalBudgetLabel.ForeColor = budgetManager.TotalBudget >= 0 
                ? System.Drawing.Color.Green 
                : System.Drawing.Color.Red;
        }

        private void ClearInputFields()
        {
            descriptionTextBox.Clear();
            amountTextBox.Clear();
            typeComboBox.SelectedIndex = 0;
            datePicker.Value = DateTime.Now;
        }

        private void AddTransactionButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(descriptionTextBox.Text))
            {
                MessageBox.Show("Введите описание транзакции!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(amountTextBox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму (положительное число)!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TransactionType type = typeComboBox.SelectedIndex == 0 ? 
                TransactionType.Доход : TransactionType.Расход;

            budgetManager.AddTransaction(new Transaction(descriptionTextBox.Text, amount, type, datePicker.Value));
            UpdateTransactionsList();
            UpdateTotalBudget();
            ClearInputFields();
            MessageBox.Show("Транзакция успешно добавлена!", "Успех", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RemoveTransactionButton_Click(object sender, EventArgs e)
        {
            if (transactionsListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите транзакцию для удаления!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Вы действительно хотите удалить?", 
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var t = budgetManager.Transactions[transactionsListBox.SelectedIndex];
                budgetManager.RemoveTransaction(t);
                UpdateTransactionsList();
                UpdateTotalBudget();
                MessageBox.Show("Транзакция удалена!", "Успех", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateTransactionButton_Click(object sender, EventArgs e)
        {
            if (transactionsListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите транзакцию для обновления!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(descriptionTextBox.Text))
            {
                MessageBox.Show("Введите новое описание!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(amountTextBox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TransactionType type = typeComboBox.SelectedIndex == 0 ? 
                TransactionType.Доход : TransactionType.Расход;

            var t = budgetManager.Transactions[transactionsListBox.SelectedIndex];
            budgetManager.UpdateTransaction(t, descriptionTextBox.Text, amount, type);
            UpdateTransactionsList();
            UpdateTotalBudget();
            ClearInputFields();
            MessageBox.Show("Транзакция обновлена!", "Успех", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}