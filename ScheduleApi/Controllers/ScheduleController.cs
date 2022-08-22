using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleApi.Data;
using ScheduleApi.Models;

namespace ScheduleApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase {
        private readonly ScheduleDbContext _context;
        public ScheduleController(ScheduleDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Schedule>> Get() {
            return await _context.Schedules.ToListAsync();
        }

        [HttpGet("weekly-schedule")]
        [ProducesResponseType(typeof(IEnumerable<Schedule>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWeeklySchedule(int week, int year) {
            try {
                var schedules = await _context.Schedules.Where(x =>
                x.Week == week && x.Year == year).ToListAsync();

                return schedules == null ? NotFound() : Ok(schedules);
            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("employee-weekly-schedule")]
        [ProducesResponseType(typeof(IEnumerable<Schedule>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEmployeeWeeklySchedule(int id, int week, int year) {
            try {
                var schedules = await _context.Schedules.Where(x =>
                x.EmployeeId == id && x.Week == week && x.Year == year).ToListAsync();

                return schedules == null ? NotFound() : Ok(schedules);
            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("employee-day-schedule")]
        [ProducesResponseType(typeof(IEnumerable<Schedule>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEmployeeDaySchedule(int id, int day, int week, int year) {
            try {
                var schedules = await _context.Schedules.Where(x =>
                x.Day == day && x.EmployeeId == id && x.Week == week && x.Year == year).ToListAsync();

                return schedules == null ? NotFound() : Ok(schedules);
            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(int id, int day, int week, int year) {
            try {
                Schedule schedule = new Schedule(id, day, week, year);
                await _context.Schedules.AddAsync(schedule);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEmployeeDaySchedule),
                    new { EmployeeId = id, Day = day, Week = week, Year = year },
                    schedule);
            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSchedule(int id, Schedule schedule) {
            if (id != schedule.ID) return BadRequest();

            try {
                _context.Entry(schedule).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            } catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
