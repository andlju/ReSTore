using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ReSTore.Web.Controllers
{

    public class HyperLink
    {
        public string Rel { get; set; }
        public string Href { get; set; }
    }

    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IEnumerable<HyperLink> Links { get; set; }
    }

    public class CustomersController : ApiController
    {
        public HttpResponseMessage Post(Customer customer)
        {
            // TODO Actually save the Customer
            var id = 1234;

            var response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri("/api/customer/" + id, UriKind.Relative);

            return response;
        }

        public Customer Get(int id)
        {
            var cust = new Customer()
                {
                    FirstName = "Anders",
                    LastName = "Ljusberg",
                    Links = new[]
                        {
                            new HyperLink() {Rel = "self", Href = "/api/customer/" + id},
                            new HyperLink() {Rel = "orders", Href = "/api/customer/" + id + "orders"},
                        }
                };
            
            return cust;
        }
    }
}