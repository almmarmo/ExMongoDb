using Autofac;
using NoSql.Abstractions;
using NoSql.MongoDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TesteMongo
{
    class Program
    {
        private static readonly string DefaultDatabase = "SECTOR";
        private static readonly string TableName = "Project.Name";
        private static Random random = new Random();
        static void Main(string[] args)
        {

            ContainerBuilder builder = new ContainerBuilder();
            builder.AddMongoDB(new MongoDBOptions("localhost", 27017, DefaultDatabase));
            var container = builder.Build();
            var id = new Guid("50c2cd8a-8492-4648-8647-cd9cb9cd9736");// "5ba91d978731417d6798139e";

            IConnection connection = container.Resolve<IConnection>();
                //Task.Run(() => connection.DeleteAllAsync<Request>())
                //    .GetAwaiter()
                //    .GetResult();

            Task.Run(() => connection.AddAsync(new AuditRequest() { AuditId = random.Next() }, TableName, x => x.AuditId)).Wait();
            Task.Run(() => connection.AddAsync(new AuditRequest() { AuditId = random.Next() }, TableName, x => x.AuditId)).Wait();

            //var req = connection.Get<Request>(id.ToString(), "DocStore");

            //Task.Run(() =>
            //connection.AddAsync(new Pessoa() { Nome = "TESTE" }, TableName, x => x.Nome))
            //.Wait(); ;

            var newObj = new Request()
            {
                teste = "dsalkfsadlfjk",
                //Pessoa = new Pessoa() { Nome = "Testando da Silva" }
            };
            Task.Run(() =>
            connection.AddAsync(newObj, TableName, x => x.RequestId))
                .Wait();
            var newObj2 = new Request()
            {
                teste = "dsalkfs",
                Pessoa = new Pessoa() { Nome = "Deletar" }
            };
            Task.Run(() => connection.AddAsync(newObj2, TableName, x => x.RequestId))
                .Wait();
            var newObj3 = new Request()
            {
                teste = "dsalkfs",
                Pessoa = new Pessoa() { Nome = "Atualizar" }
            };
            Task.Run(() => connection.AddAsync(newObj3, TableName, x => x.RequestId))
                .Wait();

            Task.Run(() =>
            connection.DeleteContentAsync(TableName, "'Pessoa.Nome':'Deletar'"))
            .Wait();

            var docs = connection.List<Request>(TableName, x => x.teste == "dsalkfsadlfjk");
            //var d = docs.FirstOrDefault();
            //Task.Run(() => connection.DeleteAsync<Request>(x => x.Pessoa.Nome == "Deletar"))
            //    .GetAwaiter()
            //    .GetResult();
            //newObj.teste = "teste de update... na verdade um replace.";
            //Task.Run(()=> 
            //    connection.UpdateAsync(newObj, x => x.Id == newObj.Id))
            //    .GetAwaiter()
            //    .GetResult();

            var t = connection.Get<Pessoa>("TESTE", TableName);
            //var t2 = connection.List<AuditRequest>(TableName, "'AuditId':2022521873");

            Console.WriteLine("Hello World!");
        }
    }
}
