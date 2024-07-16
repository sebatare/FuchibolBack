using Fuchibol.ChatService.Hubs;
using Fuchibol.ChatService.Models;
using TableDependency.SqlClient;

namespace Fuchibol.ChatService.SubscribeTableDependencies
{
    public class SubscribeUserTableDependency
    {
        SqlTableDependency<User> tableDependency;
        UserHub userHub;

        public SubscribeUserTableDependency(UserHub userHub){
            this.userHub = userHub;
        }

        public void SubscribeTableDependency()
        {
            
            string connectionString = "Data Source=SEBASTIAN;Initial Catalog=fuchibol;User Id=sa;Password=admin; Integrated Security=true";
            tableDependency = new SqlTableDependency<User>(connectionString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();

        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(User)} SqlTableDependecy error:{e.Error.Message}");
        }

        private void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<User> e)
        {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                userHub.SendUsers();
            }
        }

    }
}