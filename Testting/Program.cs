// See https://aka.ms/new-console-template for more information
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Testting;

Console.WriteLine("Hello, World!");

var context = new minimaldbContext();

//context.Database.EnsureCreated();
//ModelProp my = new ModelProp();
//my.Id = 1;
//my.Name = "Abhishek";
//my.Version = 20;
//string json = JsonConvert.SerializeObject(my);
//Console.WriteLine(json);
//string unk = "{\"Id\":5,\"Version\":12}";
//Console.WriteLine(unk);

//dynamic obj=JsonConvert.DeserializeObject(unk);
//Console.WriteLine(obj["Id"].GetType());
var indb = await context.ModelProps.FindAsync(6);
var ver = indb.GetType().GetProperty("Version").GetValue(indb).GetType();
Console.WriteLine(ver);
//indb.Name = "Zoro";
//context.SaveChanges();
//Console.WriteLine(indb.Name+","+indb.Version);
//Console.WriteLine(indb.GetType().GetProperty("Name").GetValue(indb,null));

//dynamic v = 2;
//Console.WriteLine(v.GetType());
//var jsonlist = JsonConvert.DeserializeObject<Dictionary<string,object>>(unk);

//foreach(var item in jsonlist)
//{
//    var key=item.Key;
//    var cur=item.Value;
//    Console.WriteLine(key.GetType()+",   "+cur.GetType());
//}

//using (var reader = new JsonTextReader(new StringReader(unk)))
//{
//    int i = 0;
//    while (reader.Read())
//    {
//        Console.WriteLine(i++);
//        Console.WriteLine("{0} - {1} - {2}",
//                          reader.TokenType, reader.ValueType, reader.Value);
//    }
//}