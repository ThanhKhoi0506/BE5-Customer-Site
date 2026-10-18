using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MaSV_MyStore.Models.ViewModel
{
    public class CheckoutVM
    {
        public List<CartItem> CartItems { get; set; }

        [Display(Name = "Họ tên")]
        public string CustomerName { get; set; }
        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình thức giao hàng")]
        [Display(Name = "Hình thức giao hàng")]
        public string DeliveryMethod { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình thức thanh toán")]
        [Display(Name = "Hình thức thanh toán")]
        public string PaymentMethod { get; set; }

        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
