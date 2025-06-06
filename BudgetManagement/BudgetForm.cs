﻿using System;
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

        public void RemoveText(object sender, EventArgs e)
        {
            var send = (TextBox)sender;
            string text = send.Tag.ToString();
            if (send.Text == text)
            {
                send.Text = "";
            }
        }
        public void AddText(object sender, EventArgs e)
        {
            var send = (TextBox)sender;
            string text = send.Tag.ToString();
            if (string.IsNullOrWhiteSpace(send.Text))
                send.Text = text;
        }

        public BudgetForm()
        {
            this.Text = "Управление бюджетом";
            this.Width = 600;
            this.Height = 500;
            descriptionTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 10),
                Width = 150,
                Text = "Описание",
                Tag = "Описание"
            };
            descriptionTextBox.GotFocus += new EventHandler(RemoveText);
            descriptionTextBox.LostFocus += new EventHandler(AddText);
            amountTextBox = new TextBox
            {
                Location = new System.Drawing.Point(170, 10),
                Width = 100,
                Text = "Сумма",
                Tag = "Сумма"
            };
            amountTextBox.GotFocus += new EventHandler(RemoveText);
            amountTextBox.LostFocus += new EventHandler(AddText);
            typeComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(280, 10),
                Width = 100,
                Items = { "Доход", "Расход" }
            };
            datePicker = new DateTimePicker
            {
                Location = new System.Drawing.Point(390, 10)
            };
            addTransactionButton = new Button
            {
                Location = new System.Drawing.Point(10, 40),
                Text = "Добавить",
                Width = 100
            };
            addTransactionButton.Click += AddTransactionButton_Click;
            removeTransactionButton = new Button
            {
                Location = new System.Drawing.Point(120, 40),
                Text = "Удалить",
                Width = 100
            };
            removeTransactionButton.Click += RemoveTransactionButton_Click;
            updateTransactionButton = new Button
            {
                Location = new System.Drawing.Point(220, 40),
                Text = "Обновить",
                Width = 120
            };
            updateTransactionButton.Click += UpdateTransactionButton_Click;
            transactionsListBox = new ListBox
            {
                Location = new System.Drawing.Point(10, 70),
                Width = 560,
                Height = 200
            };
            totalBudgetLabel = new Label
            {
                Location = new System.Drawing.Point(10, 280),
                Width = 200,
                Text = "Общий бюджет: "
            };
            this.Controls.Add(descriptionTextBox);
            this.Controls.Add(amountTextBox);
            this.Controls.Add(typeComboBox);
            this.Controls.Add(datePicker);
            this.Controls.Add(addTransactionButton);
            this.Controls.Add(removeTransactionButton);
            this.Controls.Add(updateTransactionButton);
            this.Controls.Add(transactionsListBox);
            this.Controls.Add(totalBudgetLabel);
            typeComboBox.Text = "Доход";
            budgetManager = new BudgetManager();
            UpdateTransactionsList();
            UpdateTotalBudget();
        }
        private void UpdateTransactionsList()
        {
            transactionsListBox.Items.Clear();
            foreach (var transaction in budgetManager.Transactions)
            {
                string type = transaction.Type == TransactionType.Доход ? "Доход" : "Расход";
                transactionsListBox.Items.Add($"{transaction.Description} - {type} ({ transaction.Amount} руб.)");
            }
        }
        private void UpdateTotalBudget()
        {
            totalBudgetLabel.Text = $"Общий бюджет: {budgetManager.TotalBudget} руб.";
        }
        private void AddTransactionButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(descriptionTextBox.Text) ||
            string.IsNullOrEmpty(amountTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }
            decimal amount;
            if (!decimal.TryParse(amountTextBox.Text, out amount) || amount <= 0)
            {
                MessageBox.Show("Неверная сумма!");
                return;
            }
            TransactionType type = (TransactionType)Enum.Parse(typeof(TransactionType),
            typeComboBox.SelectedItem.ToString());
            DateTime date = datePicker.Value;
            Transaction newTransaction = new Transaction(descriptionTextBox.Text, amount, type,
            date);
            try
            {
                budgetManager.AddTransaction(newTransaction);
                descriptionTextBox.Text = "Описание";
                amountTextBox.Text = "Сумма";
                UpdateTransactionsList();
                UpdateTotalBudget();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void RemoveTransactionButton_Click(object sender, EventArgs e)
        {
            if (transactionsListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите транзакцию для удаления!");
                return;
            }
            string selectedItem = transactionsListBox.SelectedItem.ToString();
            string[] parts = selectedItem.Split(new[] { '-' }, StringSplitOptions.None);
            if (parts.Length >= 2)
            {
                string description = parts[0].Trim();
                string amount = parts[1].Split('(')[1].Replace(" руб.)", "");
                decimal amountValue;
                if (decimal.TryParse(amount, out amountValue))
                {
                    var transactionToRemove = budgetManager.Transactions.Find(t => t.Description
                    == description && t.Amount == amountValue);
                    if (transactionToRemove != null)
                    {
                        try
                        {
                            budgetManager.RemoveTransaction(transactionToRemove);
                            UpdateTransactionsList();
                            UpdateTotalBudget();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }
        private void UpdateTransactionButton_Click(object sender, EventArgs e)
        {
            if (transactionsListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите транзакцию для обновления!");
                return;
            }
            string selectedItem = transactionsListBox.SelectedItem.ToString();
            string[] parts = selectedItem.Split(new[] { '-' }, StringSplitOptions.None);
            if (parts.Length >= 2)
            {
                string description = parts[0].Trim();
                string amount = parts[1].Split('(')[1].Replace(" руб.)", "");
                decimal amountValue;
                if (decimal.TryParse(amount, out amountValue))
                {
                    var transactionToUpdate = budgetManager.Transactions.Find(t => t.Description ==
                    description && t.Amount == amountValue);
                    if (transactionToUpdate != null)
                    {
                        if (string.IsNullOrEmpty(descriptionTextBox.Text) ||
                        string.IsNullOrEmpty(amountTextBox.Text))
                        {
                            MessageBox.Show("Заполните все поля!");
                            return;
                        }
                        decimal newAmount;
                        if (!decimal.TryParse(amountTextBox.Text, out newAmount) || newAmount <= 0)
                        {
                            MessageBox.Show("Неверная сумма!");
                            return;
                        }
                        TransactionType newType =
                        (TransactionType)Enum.Parse(typeof(TransactionType),
                        typeComboBox.SelectedItem.ToString());
                        try
                        {
                            budgetManager.UpdateTransaction(transactionToUpdate,
                            descriptionTextBox.Text, newAmount, newType);
                            UpdateTransactionsList();
                            UpdateTotalBudget();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
