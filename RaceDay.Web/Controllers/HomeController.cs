using Microsoft.AspNetCore.Mvc;
using RaceDay.Web;
using System.Text.Json;
namespace RaceDay.Web.Controllers;
public class HomeController(ApiClient api) : Controller {
 private bool SignedIn => HttpContext.Session.GetString("Role") != null;
 private IActionResult GoLogin() => RedirectToAction("Login");
 private void Feedback(string message) => TempData["Message"] = message;
 public async Task<IActionResult> Index(string? search) {
   var r=await api.Send<List<JsonElement>>(HttpMethod.Get,"api/events");
   ViewBag.Error=r.error;
   var items=r.data ?? new();
   if (!string.IsNullOrWhiteSpace(search)) items=items.Where(e=>e.GetProperty("name").GetString()!.Contains(search,StringComparison.OrdinalIgnoreCase)).ToList();
   return View(items);
 }
 [HttpGet] public IActionResult Login()=>View("Account");
 [HttpGet] public IActionResult Register()=>View("Account");
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Login(string email,string password){
   var r=await api.Send<JsonElement>(HttpMethod.Post,"api/auth/login",new {email,password});
   if (!r.ok){Feedback(r.error);return View("Account");}
   HttpContext.Session.SetString("Role",r.data.GetProperty("role").GetString()!);
   HttpContext.Session.SetString("Name",r.data.GetProperty("firstName").GetString()!);
   return RedirectToAction("Dashboard");
 }
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Register(string firstName,string lastName,string email,string password,string role,string? phone){
   var r=await api.Send<JsonElement>(HttpMethod.Post,"api/auth/register",new {firstName,lastName,email,password,role=role=="Organiser"?0:1,phone});
   Feedback(r.ok?"Registration complete. Please log in.":r.error);
   return View("Account");
 }
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Logout(){await api.Send<JsonElement>(HttpMethod.Post,"api/auth/logout");HttpContext.Session.Clear();return RedirectToAction("Index");}
 public async Task<IActionResult> Dashboard(){
   if(!SignedIn)return GoLogin();
   var role=HttpContext.Session.GetString("Role");ViewBag.Role=role;
   var events=await api.Send<List<JsonElement>>(HttpMethod.Get,"api/events");ViewBag.Error=events.error;
   if(role=="Participant") {var mine=await api.Send<List<JsonElement>>(HttpMethod.Get,"api/enrolments");ViewBag.Enrolments=mine.data ?? new();}
   return View(events.data ?? new());
 }
 [HttpGet] public async Task<IActionResult> Event(int id){
   var r=await api.Send<JsonElement>(HttpMethod.Get,$"api/events/{id}");
   if(!r.ok){Feedback(r.error);return RedirectToAction("Index");}
   ViewBag.Role=HttpContext.Session.GetString("Role");
   var results=await api.Send<List<JsonElement>>(HttpMethod.Get,$"api/events/{id}/results");ViewBag.Results=results.data ?? new();
   if(ViewBag.Role=="Organiser") {var enrolments=await api.Send<List<JsonElement>>(HttpMethod.Get,$"api/events/{id}/enrolments");ViewBag.Enrolments=enrolments.data ?? new();}
   return View(r.data);
 }
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Enrol(int id,int categoryId){
   if(HttpContext.Session.GetString("Role")!="Participant")return Forbid();
   var r=await api.Send<JsonElement>(HttpMethod.Post,$"api/events/{id}/enrolments",new {categoryId});Feedback(r.ok?"Enrolment submitted.":r.error);return RedirectToAction("Event",new{id});
 }
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Cancel(int enrolmentId){
   if(HttpContext.Session.GetString("Role")!="Participant")return Forbid();
   var r=await api.Send<JsonElement>(HttpMethod.Delete,$"api/enrolments/{enrolmentId}");Feedback(r.ok?"Enrolment cancelled.":r.error);return RedirectToAction("Dashboard");
 }
 [HttpGet] public IActionResult CreateEvent()=>HttpContext.Session.GetString("Role")=="Organiser"?View():Forbid();
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> CreateEvent(string name,string? description,DateTime eventDate,string location,decimal distanceKm,int eventType,DateTime registrationOpen,DateTime registrationClose,int status){
   if(HttpContext.Session.GetString("Role")!="Organiser")return Forbid();
   var r=await api.Send<JsonElement>(HttpMethod.Post,"api/events",new{name,description,eventDate,location,distanceKm,eventType,registrationOpen,registrationClose,status});
   Feedback(r.ok?"Event created.":r.error);return RedirectToAction("Dashboard");
 }
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> DeleteEvent(int id){
   if(HttpContext.Session.GetString("Role")!="Organiser")return Forbid();
   var r=await api.Send<JsonElement>(HttpMethod.Delete,$"api/events/{id}");Feedback(r.ok?"Event deleted.":r.error);return RedirectToAction("Dashboard");
 }
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> PublishResult(int id,int enrolmentId,string? finishTime,int? position,int resultStatus){
   if(HttpContext.Session.GetString("Role")!="Organiser")return Forbid();
   TimeSpan? duration=TimeSpan.TryParse(finishTime,out var t)?t:null;
   var r=await api.Send<JsonElement>(HttpMethod.Post,$"api/events/{id}/results",new{enrolmentId,finishTime=duration,position,resultStatus});
   Feedback(r.ok?"Result saved.":r.error);return RedirectToAction("Event",new{id});
 }
 [HttpGet] public async Task<IActionResult> Profile(){if(!SignedIn)return GoLogin();var r=await api.Send<JsonElement>(HttpMethod.Get,"api/profile");if(!r.ok){Feedback(r.error);return RedirectToAction("Dashboard");}return View(r.data);}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Profile(string firstName,string lastName,string? phone,string? gender,string? emergencyContactName,string? emergencyContactPhone,string? address){
   if(!SignedIn)return GoLogin();var r=await api.Send<JsonElement>(HttpMethod.Put,"api/profile",new{firstName,lastName,phone,gender,emergencyContactName,emergencyContactPhone,address});Feedback(r.ok?"Profile updated.":r.error);return RedirectToAction("Profile");
 }
 public IActionResult Error()=>View("Error");
}
