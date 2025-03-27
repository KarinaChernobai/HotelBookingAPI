using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HotelBookingAPI.Models;
using HotelBookingAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HotelBookingAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HotelBookingController : ControllerBase
    {
        private readonly ApiContext _context;
        public HotelBookingController(ApiContext context)
        {
            _context = context;
            context.Database.
        }

        // Create/Edit
        [HttpPost]
        public JsonResult CreateEdit(HotelBooking booking)
        {
            var p = Expression.Parameter(typeof(HotelBooking), "b");
            var predicate1 = (Expression<Func<HotelBooking, bool>>)Expression.Lambda
            (
                Expression.GreaterThan(Expression.Property(p, "RoomNumber"), Expression.Constant(200)),
                p
            );
            var predicate2 = (Expression<Func<HotelBooking, bool>>)(b => b.RoomNumber > 200);
            var predicate3 = (Func<HotelBooking, bool>)(b => b.RoomNumber > 200);

            if (booking.Id == 0)
            {
                _context.Bookings.Add(booking);
            }
            else
            {
                var bookingInDb = _context.Bookings.Find(booking.Id);
                if (bookingInDb == null) return new JsonResult(NotFound());
                _context.Bookings.Entry(bookingInDb).State = EntityState.Detached;
                _context.Bookings.Update(booking);
                //bookingInDb.RoomNumber = booking.RoomNumber;
                //bookingInDb.ClientName = booking.ClientName;
                var resSet = from b in _context.Bookings where b.RoomNumber > 200 select b.ClientName;
                var resSetN = from r in resSet where r.Length > 3 select r;
                var list = resSetN.ToList();
                var resSet2 = _context.Bookings.Where(predicate1).Select(b => b.ClientName).ToList();
            }

            _context.SaveChanges();
            return new JsonResult(Ok(booking));
        }

        // Get
        [HttpGet]
        public JsonResult Get (int id)
        {
            var result = _context.Bookings.Find(id);
            if (result == null) return new JsonResult(NotFound());
            return new JsonResult(Ok(result));
        }

        // Delete
        [HttpDelete]
        public JsonResult Delete (int id)
        {
            var result = _context.Bookings.Find(id);
            if(result == null) return new JsonResult(NotFound());
            _context.Bookings.Remove(result);
            _context.SaveChanges();
            return new JsonResult(NoContent());
        }

        // Get all
        [HttpGet()]
        public JsonResult GetAll()
        {
            var result = _context.Bookings.ToList();
            return new JsonResult(Ok(result));
        }
    }
}
