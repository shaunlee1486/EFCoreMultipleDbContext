- different sql databases
- read replica
- modular monolith

```
let's me comment on when you might want to use separate database contect in a single application there are three common use cases that I've seen 
- the first use case when you have different sql database for the same application for example one database could be sql server but the other database could be a postgresql database so you would need to create a different database context to talk to to these different databases 

- the Second Use case that I've seen is when you want to use a read replica for one of your databases so this is just a copy of your main database but it's working in a read-only mode in that case you can configure your database context to work in read-only mode by calling the  use Query tracking Behavior (1) method I can then pass the no tracking Behavior by default and all of my queries are going to be no tracking  when I'm reading from the database and this is something that you want in read-only mode alternatively you can also configure this from the  database context by overriding the on configuring method  and you have access to the options Builder and again you can say use Query tracking behavior (2) and specify no tracking so this is another alternative but I'm going to leave this out because I don't want to be working in a null tracking Manner and 

- the third use case that I've seen for multiple database context is when you are using a modular monolith when you are buildings a modular monolith each module will have one database context the separation at the database level could be by schema or event physically separated databases but regardless of that you absolutely have to use different database contexts if you want to build a proper modular monolith if I leave my database context like this because I'm connecting to the same database they're going to live in the same schema now I don't necessarily want to do this and let's implement a form of logical separation at a dataase level by saying mono Builder has default schema (3) and I'm going to say orders so now all of the tables in the orders database context are going to live in the orders schema and I'm going to also update the products database context to place the tables in the product schema the next thing I want to do is to create a migration for my database context so I'm going to open up the package manager console and let me try to add a Migration by saying add a migration and I'll call it create database s if I run this command  you're going to see that we get an error saying that we have more than one database context in the application so we have to specify the context using the context parameter to Target the specific database context that we want to create a migration for so I'm going to say add migration create datababse and let's target on of  the context that we have so I'll use ordersdbcontext (4) and by default the migration is going to live in a migrations folder that this tool is going to create this also means that we're going to mix the migration between our database context and this isn't what I want so I'm going to define the out put folder and let's call it migrations Dash orders so if I run this you're going to see that we get a migration and it's going to live inside of the orders folder in the migration root folder so here's why what our migration looks like we have the order schema then we have the orders table the Align items table and everything that we need to make this functional let's also specify the same migration for the products database context I'm going to update the first parameter to be my productDbContext (5) and the output folder the migration is also going to be migration slash products so if I create this we're going to get another migration is going to be in the product schema and we only have one table inside which is our products table notice also that this migration is going to seed our initial products so that we have some initial data to work with when we start the application if I want to apply these migrations on the database level I'm going to use the update database command so I'll say update database again I need to specify the specify the specific contacts that I want to use so I'll say update database for the orderdbcontext (6) and when I run this it's going to to apply the migration let's do the same for the productDBcontext(7) now I'm doing this because I want to show you one thing that might not be obvious and you're going to see it when I show you the database so let's move over to my SQL Server Management Studio the migrations I applied were referencing the webshop database and if I open up the tables inside you're going to see that we get the orders table the line items table and the products table all in their respective schema if we select what's inside the products table you'll see that we have the five products that defined as my initial data but you'll notice that the migration history table is not in its own schema it's going to contain the migrations for both of your database contexts and this might not be what you want to get so I'm going to show you how to move the migration history to be separate for every database context if I head over to the call to use SQL Server where we were passing the connection string I can also specify on more argument for the SQL Server database context options Builder (8) so that's a really long name but I can specify an action and on this section I have the migration history table method this allows me to specify the table name and the schema for the migrations history table and by default the name this table will live in the history repository default table name and because I'm using the orderdbcontext I can specify the schema as orders and this is how you can move your migration's history table into its own schema in this case it's going to match what we already have for the other tables in this database context I'm also going to update the product database conext (9) and now I'm going to apply the migrations again and I deleted the tables in the background so these are going to be applied on the fresh database so we're going to run the orders migrations and the products migrations and then we're going to check our SQL Server instance so if I head over there and I refresh the table that I have here this time you'll see that our EF migration history table is in the correct schema so for the products context we have one migration history and for orders context we have another migration history so this is something that you definitely want to have if you're using multiple database contexts in a single database and I also want to show you how to actually use multiple database contexts from your application so let's start by defining some minimal API endpoints so this one will be with the route of products (10) it's going to bbe asynchronous in the request delegate and I'm going to access the product DB context so let's call this product context and I'm going to use it to return my response so I'm going to say return results dot okay and I'm going to await product DB context access my product I'm going to select the productID and I'm going to cast this to an array so I'm just going to return the identifiers of all of the product in the database and then I'm going to pass them to another endpoint for creating an order so I'm going to define a contracts folder which is going to contain one class inside and this will be my submit order request and all that I want on this class is just a list of goods which is going to match my productID and I'm going to call it product IPS let's also assign this to a default empty list so now we're gong to use this to create one more endpoint this is going to be a post endpoint (11) with a route of orders and the request delegate is going to be a bit more involved so first of all I need my submite order request to create my order then I'm gong to need the products DB context to be able to check if these products exist in the database I'm going to start moving them into separate rows so that you can see everything on the screen then I'm going to need my ordersdb context so this will be the orders context and I can Define the body of my endpoint and then what I'm gong to do inside of the this endpoint is first fetch the products so I'm going to say product context, products where the productID is inside of the list that we pass in request so request productsID contains the productId I'm going to say has no tracking and let's load all of the products to memory so to list async of course I need to await this because this is an asynchronous method and let's do some simple validation I'm going to say if products count is not equal to the request productIds count then let's return a bad request so I'm going to save results by request some product is missing I'm not going to deal with which specific product is missing now let's create our order instance so I'm going to say New Order and let's assign the properties so the ID will be a new good the total price of the order I'm going to get by iterating over the products using the sum method and just adding to together the individual prices for each product and then for the light items I'm also going to iterate over the products but I'm going to project them into a new line item instance so i'm going to say new line item and let's set the line item properties so the good is going to be a new good the product ID will be from the product instance then we have the price also from our product and the order Id is going to be set automatically by EF core and I also need to call tolist for this to match what the line items property is expecting so let me just format this and we have our order and now I can orders context and we add the order to the databbase context I'm going to to save this by calling orderscontext save changes async and I'm just going to return this order from this endpoint so let's return the order to the user so that they can see what they created there are two constraints when you are working with multiple database contexts the first constraint is that you can't define an explicit join between the database sets or the tables on these database context this because any framework core does not know if these tables live in the same database so even if this is true under the hood EF core doesn't know so it's not allowed the second thing is about transactions and they're only going to work if you're actually using the same database but you have to tell EF core to actually use the specific transaction how you can do this is let's say I create a new transaction  so (12) using var transaction and let's create a new SQL transaction I'm not going to specify the connection stirng now how you will tell the database context to use it is by saying order's context database use transaction (12) and then you pass it the transaction instance and this is how you can share a transaction between multiple database context which means you would also have to do the same for the products context use this transaction and on the orders context use the same transaction and then both of the contexts will in the same database transaction but I repeat this only works if they are using the same physical database under the hood now I'm going to get red of this and I'm going to show you how this is actually working so let's start the application so I'm going to use my products get endpoint to get my array of product IDs so these are just the ones that we see that in our database migration and I'm gong to pass them to my post endpoint to add them to the order so I'm going to pass these to my orders endpoint and it's going to fetch the products from the products DB context and then use the orders database context to create my order and you can see that we get back an order with a list of line items and all of the properties are matching what we wrote in 

```

1. builder.Services.AddDbContext<ProductsDbContext>(options => options.UseSqlServer(connectString)
			.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
			1. 
2. protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
		}
		1. 
3. modelBuilder.HasDefaultSchema("orders");

4. add-migration Create_Database -Context OrdersDbContext -o Migrations/Orders
5. add-migration Create_Database -Context ProductsDbContext -o Migrations/Products

6. update-database -Context OrdersDbContext
7. update-database -Context ProductsDbContext

8. builder.Services.AddDbContext<OrdersDbContext>(options => options.UseSqlServer(
				connectString, 
				o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "orders")));
				1. 
9. builder.Services.AddDbContext<ProductsDbContext>(options => options.UseSqlServer(
				connectString,
				o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "products")));

10. app.MapGet("products", async (ProductsDbContext productsDbContext) =>
			{
				return Results.Ok(await productsDbContext.Products.Select(p => p.Id).ToArrayAsync());
			});

11. app.MapPost("orders", async (
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
				

12. using var transaction = new SqlTransaction();	productsDbContext.Database.UseTransaction(transaction);
	ordersDbContext.Database.UseTransaction(transaction);

13. 
