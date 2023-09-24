namespace WebShop.API.Contracts
{
	public class SubmitOrderRequest
	{
		public List<Guid> ProductIds { get; set; } = new();
    }
}
