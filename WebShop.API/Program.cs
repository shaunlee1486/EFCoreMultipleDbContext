using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using WebShop.API.Contracts;
using WebShop.API.Orders;
using WebShop.API.Products;

namespace WebShop.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			//db context
			var connectString = builder.Configuration.GetConnectionString("DefaultContext");

			builder.Services.AddDbContext<OrdersDbContext>(options => options.UseSqlServer(
				connectString,
				o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "orders")));

			builder.Services.AddDbContext<ProductsDbContext>(options => options.UseSqlServer(
				connectString,
				o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "products")));

			var app = builder.Build();

			app.MapGet("products", async (ProductsDbContext productsDbContext) =>
			{
				return Results.Ok(await productsDbContext.Products.Select(p => p.Id).ToArrayAsync());
			});

			app.MapPost("orders", async (
				SubmitOrderRequest request,
				ProductsDbContext productsDbContext,
				OrdersDbContext ordersDbContext) =>
			{
				var products = await productsDbContext.Products
				.Where(p => request.ProductIds.Contains(p.Id))
				.AsNoTracking()
				.ToListAsync();

				if (products.Count != request.ProductIds.Count)
					return Results.BadRequest("Some product is missing");

				//using var transaction = new SqlTransaction();

				//productsDbContext.Database.UseTransaction(transaction);
				//ordersDbContext.Database.UseTransaction(transaction);

				var order = new Order
				{
					Id = Guid.NewGuid(),
					TotalPrice = products.Sum(p => p.Price),
					LineItems = products
						.Select(p => new LineItem
						{
							Id = Guid.NewGuid(),
							ProductId = p.Id,
							Price = p.Price
						})
						.ToList()
				};

				await ordersDbContext.SaveChangesAsync();

				return Results.Ok(order);
			});
			app.UseHttpsRedirection();

			app.Run();
		}
	}
}