using System.ComponentModel.DataAnnotations;

namespace TechStore.Domain.DTOs.Cart;

public class UpdateCartItemQuantityDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}
