using Exemple.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Data.Models
{
    public class CosDto
    {
        public int id { get; set; }
        public string OrderID { get; set; }
        public string ProductID { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public string Address { get; set; }

        public static CosDto ToCosDTO(int orderID, CalculatedPrice products)
        {
            return new CosDto()
            {
                OrderID = orderID.ToString(),
                ProductID = products.productCode.ToString(),
                Quantity = Convert.ToInt32(products.quantity),
                Price = products.price,
                Address = products.address.ToString()
            };
        }
        /*
         CREATE TABLE CosDta (
                ID int IDENTITY(1,1) PRIMARY KEY,
                OrderID varchar(255),
                ProductID varchar(255),
                Quantity int,
                Price int,
                Address varchar(255) 
                );    
        
  
          DROP TRIGGER IF EXISTS orderViewTrigger;
          GO
          CREATE TRIGGER  orderViewTrigger ON  dbo.CosDta
            AFTER INSERT
            AS
            BEGIN
	          DECLARE @OID varchar(255);
	          DECLARE @P int;
	          DECLARE @A varchar(255);
              SELECT  @OID=dbo.CosDta.OrderID, @P=SUM(dbo.CosDta.Price),@A= dbo.CosDta.Address
	          FROM dbo.CosDta group by dbo.CosDta.Address;
	          INSERT into dbo.OrderView values (@OID,@P,@A);
            END;
         */
    }
}
