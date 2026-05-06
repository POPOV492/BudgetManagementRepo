using System;
using System.Windows.Forms;

namespace BudgetManagement
{
    public partial class BudgetForm : Form
    {
        private BudgetManager budgetManager;
        private TextBox descriptionTextBox;
        private TextBox amountTextBox;
        private ComboBox typeComboBox;
        private DateTimePicker datePicker;
        private Button addTransactionButton;
        private Button removeTransactionButton;
        private Button updateTransactionButton;
        private ListBox transactionsListBox;
        private Label totalBudgetLabel;

        public BudgetForm()
        {
            InitializeComponents();
            budgetManager = new BudgetManager();
            UpdateTransactionsList();
            UpdateTotalBudget();
        }

        private void InitializeComponents()
        {
            this.Text = "Управление бюджетом";
            this.Width = 600;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;

            descriptionTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 10),
                Width = 150,
                PlaceholderText = "Описание"
            };

            amountTextBox = new TextBox
            {
                Location = new System.Drawing.Point(170, 10),
                Width = 100,
                PlaceholderText = "Сумма"
            };

            typeComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(280, 10),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            typeComboBox.Items.Add("Доход");
            typeComboBox.Items.Add("Расход");
            typeComboBox.SelectedIndex = 0;

            datePicker = new DateTimePicker
            {
                Location = new System.Drawing.Point(390, 10),
                Width = 150,
                Format = DateTimePickerFormat.Short
            };

            addTransactionButton = new Button
            {
                Location = new System.Drawing.Point(10, 40),
                Text = "Добавить",
                Width = 100,
                BackColor = System.Drawing.Color.LightGreen
            };
            addTransactionButton.Click += AddTransactionButton_Click;

            removeTransactionButton = new Button
            {
                Location = new System.Drawing.Point(120, 40),
                Text = "Удалить",
                Width = 100,
                BackColor = System.Drawing.Color.LightCoral
            };
            removeTransactionButton.Click += RemoveTransactionButton_Click;

            updateTransactionButton = new Button
            {
                Location = new System.Drawing.Point(230, 40),
                Text = "Обновить",
                Width = 100,
                BackColor = System.Drawing.Color.LightBlue
            };
            updateTransactionButton.Click += UpdateTransactionButton_Click;

            transactionsListBox = new ListBox
            {
                Location = new System.Drawing.Point(10, 80),
                Width = 560,
                Height = 250,
                SelectionMode = SelectionMode.One
            };

            totalBudgetLabel = new Label
            {
                Location = new System.Drawing.Point(10, 340),
                Width = 300,
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold),
                Text = "Общий бюджет: 0 руб."
            };

            this.Controls.AddRange(new Control[] {
                descriptionTextBox,
                amountTextBox,
                typeComboBox,
                datePicker,
                addTransactionButton,
                removeTransactionButton,
                updateTransactionButton,
                transactionsListBox,
                totalBudgetLabel
            });
        }

        private void UpdateTransactionsList()
        {
            transactionsListBox.Items.Clear();
            foreach (var transaction in budgetManager.Transactions)
            {
                string type = transaction.Type == TransactionType.Доход ? "+" : "-";
                string displayText = $"{transaction.Description} | {type} {transaction.Amount} руб. | {transaction.Date:dd.MM.yyyy}";
                transactionsListBox.Items.Add(displayText);
            }
        }

        private void UpdateTotalBudget()
        {
            totalBudgetLabel.Text = $"Общий бюджет: {budgetManager.TotalBudget} руб.";
            if (budgetManager.TotalBudget >= 0)
                totalBudgetLabel.ForeColor = System.Drawing.Color.Green;
            else
                totalBudgetLabel.ForeColor = System.Drawing.Color.Red;
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
            try
            {
                if (string.IsNullOrWhiteSpace(descriptionTextBox.Text))
                {
                    MessageBox.Show("Введите описание транзакции!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(amountTextBox.Text))
                {
                    MessageBox.Show("Введите сумму!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(amountTextBox.Text, out decimal amount))
                {
                    MessageBox.Show("Введите корректную сумму (число)!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (amount <= 0)
                {
                    MessageBox.Show("Сумма должна быть положительной!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                TransactionType type = typeComboBox.SelectedIndex == 0 ?
                    TransactionType.Доход : TransactionType.Расход;

                Transaction newTransaction = new Transaction(
                    descriptionTextBox.Text,
                    amount,
                    type,
                    datePicker.Value
                );

                budgetManager.AddTransaction(newTransaction);
                UpdateTransactionsList();
                UpdateTotalBudget();
                ClearInputFields();

                MessageBox.Show("Транзакция успешно добавлена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении транзакции: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoveTransactionButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (transactionsListBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите транзакцию для удаления!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show("Вы действительно хотите удалить выбранную транзакцию?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int selectedIndex = transactionsListBox.SelectedIndex;
                    var transactionToRemove = budgetManager.Transactions[selectedIndex];
                    budgetManager.RemoveTransaction(transactionToRemove);
                    UpdateTransactionsList();
                    UpdateTotalBudget();

                    MessageBox.Show("Транзакция успешно удалена!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении транзакции: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTransactionButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (transactionsListBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите транзакцию для обновления!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(descriptionTextBox.Text))
                {
                    MessageBox.Show("Введите новое описание транзакции!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(amountTextBox.Text))
                {
                    MessageBox.Show("Введите новую сумму!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(amountTextBox.Text, out decimal newAmount))
                {
                    MessageBox.Show("Введите корректную сумму (число)!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (newAmount <= 0)
                {
                    MessageBox.Show("Сумма должна быть положительной!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                TransactionType newType = typeComboBox.SelectedIndex == 0 ?
                    TransactionType.Доход : TransactionType.Расход;

                int selectedIndex = transactionsListBox.SelectedIndex;
                var transactionToUpdate = budgetManager.Transactions[selectedIndex];
                budgetManager.UpdateTransaction(transactionToUpdate, descriptionTextBox.Text, newAmount, newType);
                UpdateTransactionsList();
                UpdateTotalBudget();
                ClearInputFields();

                MessageBox.Show("Транзакция успешно обновлена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении транзакции: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}