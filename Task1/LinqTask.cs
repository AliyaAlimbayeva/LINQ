using System;
using System.Collections.Generic;
using Task1.DoNotChange;
using System.Linq;

namespace Task1
{
    public static class LinqTask
    {
        public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
        {
              return (from c in customers
                      where c.Orders.Sum(o => o.Total) > limit
                      select c);
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            return
                  (from c in customers
                   select (customer: c,
                   suppliers: from s in suppliers
                              where s.Country == c.Country && s.City == c.City
                              select s));
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            return (from c in customers
                    join s in suppliers on new { c.City, c.Country } equals new { s.City, s.Country } into joinRes
                    from j in joinRes.DefaultIfEmpty()
                    select new { cust = c, sup = j }
                     ).GroupBy(r => r.cust).Select(res => (res.Key, suppliers:
                     res.Where(s => s.sup != null).Select(s => s.sup)));

            
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            return (from c in customers
                    where c.Orders.Any(o => o.Total > limit)
                    select c);
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            return customers.Where(c => c.Orders.Length > 0).Select(cust => (cust, cust.Orders.Min(o => o.OrderDate)));
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            return Linq4(customers).OrderBy(cust => (cust.dateOfEntry.Year)).ThenBy(cust => cust.dateOfEntry.Month)
            .ThenByDescending(cust => cust.customer.Orders.Sum(order => order.Total))
            .ThenBy(cust => cust.customer.CompanyName);
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            return from c in customers
                   where c.PostalCode == null || c.PostalCode.Any(p => !char.IsDigit(p))
                                       || string.IsNullOrWhiteSpace(c.Region)
                                       || !c.Phone.StartsWith("(")
                   select c;
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            /* example of Linq7result

             category - Beverages
	            UnitsInStock - 39
		            price - 18.0000
		            price - 19.0000
	            UnitsInStock - 17
		            price - 18.0000
		            price - 19.0000
             */

            return products.GroupBy(p => p.Category).
                   Select(prod =>
                   new Linq7CategoryGroup
                   {
                       Category = prod.Key,
                       UnitsInStockGroup = prod.GroupBy(prg => prg.UnitsInStock)
                       .Select(res =>
                        new Linq7UnitsInStockGroup
                        {
                            UnitsInStock = res.Key,
                            Prices = res.Select(price => price.UnitPrice)
                        })
                   });
        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            return products.GroupBy(prod => prod.UnitPrice <= cheap ? cheap
          : prod.UnitPrice <= middle ? middle : prod.UnitPrice >= expensive ? expensive : 0)
               .Where(p => p.Key != 0)
               .Select((p) => (p.Key, products: p.Select(pr => pr)));
        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {
            return customers.GroupBy((cust) => cust.City)
                .Select(c => (city: c.Key, averageIncome: Convert.ToInt32(c.Average(cs => cs.Orders.Sum(o => o.Total))),
                 averageIntensity: Convert.ToInt32(c.Average(cc => cc.Orders.Count()))));  ;
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            return suppliers.Select(s => s.Country).Distinct().OrderBy(sup => (sup.Length))
                .ThenBy(sup => sup).Aggregate("", (current, next) => current + next);
        }
    }
}