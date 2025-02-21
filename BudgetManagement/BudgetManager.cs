using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManagement
{
    public class BudgetManager
    {
        public List<Transaction> Transactions { get; private set; }
        public decimal TotalBudget
        {
            get
            {
                return Transactions.Sum(t => t.Type == TransactionType.Доход ? t.Amount :
                -t.Amount);
            }
        }
        public BudgetManager()
        {
            Transactions = new List<Transaction>();
            LoadTransactions();
        }
        public void AddTransaction(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            Transactions.Add(transaction);
            SaveTransactions();
        }
        public void RemoveTransaction(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            Transactions.Remove(transaction);
            SaveTransactions();
        }
        public void UpdateTransaction(Transaction transaction, string newDescription, decimal
        newAmount, TransactionType newType)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            transaction.Description = newDescription;
            transaction.Amount = newAmount;
            transaction.Type = newType;
            SaveTransactions();
        }
        private void SaveTransactions()
        {
            File.WriteAllLines("transactions.txt", Transactions.Select(t =>
            $"{t.Description}|{t.Amount}|{(int)t.Type}|{t.Date.ToString("yyyy-MM-dd HH:mm:ss")}"));
        }
        private void LoadTransactions()
        {
            if (File.Exists("transactions.txt"))
            {
                var lines = File.ReadAllLines("transactions.txt");
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 4)
                    {
                        decimal amount;
                        TransactionType type;
                        DateTime date;
                        if (decimal.TryParse(parts[1], out amount) &&
                        Enum.TryParse<TransactionType>(parts[2], out type) && DateTime.TryParse(parts[3],
                        out date))
                        {
                            Transactions.Add(new Transaction(parts[0], amount, type, date));
                        }
                    }
                }
            }
        }
    }
}
