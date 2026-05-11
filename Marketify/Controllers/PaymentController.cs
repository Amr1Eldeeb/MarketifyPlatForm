using Marketify.PaymentServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymobService _paymobService;
        private readonly IConfiguration _configuration;

        public PaymentController(PaymobService paymobService, IConfiguration configuration)
        {
            _paymobService = paymobService;
            _configuration = configuration;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] decimal amount)
        {
            // 1. هات التوكن
            var token = await _paymobService.GetAuthToken();
            if (string.IsNullOrEmpty(token)) return BadRequest("فشل في الحصول على الـ Auth Token");

            // 2. سجل الطلب
            var orderId = await _paymobService.CreateOrder(token, amount);
            if (orderId == 0) return BadRequest("فشل في إنشاء الطلب (Order ID is 0)");

            // 3. هات مفتاح الدفع
            var paymentKey = await _paymobService.GetPaymentKey(token, orderId, amount);
            if (string.IsNullOrEmpty(paymentKey)) return BadRequest("فشل في الحصول على مفتاح الدفع (Payment Key is null)");

            // 4. الرابط النهائي
            var iframeId = _configuration["PaymobSettings:IframeId"];
            var paymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKey}";

            return Ok(new { url = paymentUrl });
        }
    }
}
