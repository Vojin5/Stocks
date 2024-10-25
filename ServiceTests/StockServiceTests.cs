using Entities.DTO;
using Entities.HelperClasses;
using ServiceContracts;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests
{
    public class StockServiceTests
    {
        private readonly IStocksService _stocksService;
        public StockServiceTests()
        {
            _stocksService = new StocksService();
        }

        #region CreateBuyOrder

        //When you supply BuyOrderRequest as null, it should throw ArgumentNullException.
        [Fact]
        public async Task CreateBuyOrder_NullAsRequest()
        {
            BuyOrderRequest? request = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _stocksService.CreateBuyOrder(request);
            });
        }

        //When you supply buyOrderQuantity as 0 (as per the specification, minimum is 1),
        //it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_QuantityAsZero()
        {
            BuyOrderRequest request = new BuyOrderRequest()
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 0,
                Price = 100
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateBuyOrder(request);
            });
        }

        //When you supply buyOrderQuantity as 100001
        //(as per the specification, maximum is 100000), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_QuantityOverflow()
        {
            BuyOrderRequest request = new BuyOrderRequest()
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 100001,
                Price = 100
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateBuyOrder(request);
            });
        }

        //When you supply buyOrderPrice as 0
        //(as per the specification, minimum is 1), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_PriceAsZero()
        {
            BuyOrderRequest request = new BuyOrderRequest()
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 100,
                Price = 0
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateBuyOrder(request);
            });
        }

        //When you supply buyOrderPrice as 10001
        //(as per the specification, maximum is 10000), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_PriceOverflow()
        {
            BuyOrderRequest request = new BuyOrderRequest()
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 100,
                Price = 10001
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateBuyOrder(request);
            });
        }

        //When you supply stock symbol=null (as per the specification,
        //stock symbol can't be null), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_SymbolAsNull()
        {
            BuyOrderRequest request = new BuyOrderRequest()
            {
                StockSymbol = null,
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 100,
                Price = 100
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateBuyOrder(request);
            });
        }

        //When you supply dateAndTimeOfOrder as "1999-12-31" (YYYY-MM-DD)
        //- (as per the specification,
        //it should be equal or newer date than 2000-01-01), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_WrongDate()
        {
            BuyOrderRequest request = new BuyOrderRequest()
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.Parse("1999-12-31"),
                Quantity = 100,
                Price = 100
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateBuyOrder(request);
            });
        }

        //If you supply all valid values, it should be successful and
        //return an object of BuyOrderResponse type with auto-generated BuyOrderID (guid).
        [Fact]
        public async Task CreateBuyOrder_ValidRequest()
        {
            BuyOrderRequest request = new BuyOrderRequest()
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 100,
                Price = 100
            };
            BuyOrderResponse response = await _stocksService.CreateBuyOrder(request);
            Assert.NotNull(response);
            ModelValidator.Validate(response);
        }
        #endregion

        #region CreateSellOrder

        //When you supply SellOrderRequest as null, it should throw ArgumentNullException.
        [Fact]
        public async Task CreateSellOrder_NullAsRequest()
        {
            SellOrderRequest? request = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _stocksService.CreateSellOrder(request);
            });
        }

        //When you supply buyOrderQuantity as 0 (as per the specification, minimum is 1),
        //it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_QuantityAsZero()
        {
            SellOrderRequest request = new SellOrderRequest()
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 0,
                Price = 100
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateSellOrder(request);
            });
        }

        //When you supply SellOrderQuantity as 100001
        //(as per the specification, maximum is 100000), it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_QuantityOverflow()
        {
            SellOrderRequest request = new SellOrderRequest()
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 100001,
                Price = 100
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateSellOrder(request);
            });
        }

        //When you supply SellOrderPrice as 0
        //(as per the specification, minimum is 1), it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_PriceAsZero()
        {
            SellOrderRequest request = new SellOrderRequest()
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 100,
                Price = 0
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateSellOrder(request);
            });
        }

        //When you supply SellOrderPrice as 10001
        //(as per the specification, maximum is 10000), it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_PriceOverflow()
        {
            SellOrderRequest request = new SellOrderRequest()
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 100,
                Price = 10001
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateSellOrder(request);
            });
        }

        //When you supply stock symbol=null (as per the specification,
        //stock symbol can't be null), it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_SymbolAsNull()
        {
            SellOrderRequest request = new SellOrderRequest()
            {
                StockSymbol = null,
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 100,
                Price = 100
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateSellOrder(request);
            });
        }

        //When you supply dateAndTimeOfOrder as "1999-12-31" (YYYY-MM-DD)
        //- (as per the specification,
        //it should be equal or newer date than 2000-01-01), it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_WrongDate()
        {
            SellOrderRequest request = new SellOrderRequest()
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.Parse("1999-12-31"),
                Quantity = 100,
                Price = 100
            };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateSellOrder(request);
            });
        }

        //If you supply all valid values, it should be successful and
        //return an object of SellOrderResponse type with auto-generated BuyOrderID (guid).
        [Fact]
        public async Task CreateSellOrder_ValidRequest()
        {
            SellOrderRequest request = new SellOrderRequest()
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 100,
                Price = 100
            };
            SellOrderResponse response = await _stocksService.CreateSellOrder(request);
            Assert.NotNull(response);
            ModelValidator.Validate(response);
        }

        #endregion

        #region GetAllBuyOrders

        //When you invoke this method, by default, the returned list should be empty.
        [Fact]
        public async Task GetAllBuyOrders_Default()
        {
            List<BuyOrderResponse> responseList = await _stocksService.GetBuyOrders();
            Assert.Empty(responseList);
        }

        //When you first add few buy orders using CreateBuyOrder() method;
        //and then invoke GetAllBuyOrders() method; the returned list should
        //contain all the same buy orders.
        [Fact]
        public async Task GetAllBuyOrders_FewOrders()
        {
            BuyOrderRequest request1 = new BuyOrderRequest()
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 4,
                Price = 100
            };

            BuyOrderRequest request2 = new BuyOrderRequest()
            {
                StockSymbol = "AAPL",
                StockName = "Apple Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 20,
                Price = 5000
            };

            BuyOrderResponse response1 = await _stocksService.CreateBuyOrder(request1);
            BuyOrderResponse response2 = await _stocksService.CreateBuyOrder(request2);
            List<BuyOrderResponse> expectedList = new List<BuyOrderResponse>() 
                { response1, response2 };
            
            ModelValidator.Validate(response1);
            ModelValidator.Validate(response2);

            List<BuyOrderResponse> actualList = await _stocksService.GetBuyOrders();
            foreach(BuyOrderResponse response in expectedList)
            {
                Assert.Contains(response, actualList);
            }
        }

        #endregion

        #region GetAllSellOrders

        //When you invoke this method, by default, the returned list should be empty.
        [Fact]
        public async Task GetAllSellOrders_Default()
        {
            List<SellOrderResponse> responseList = await _stocksService.GetSellOrders();
            Assert.Empty(responseList);
        }

        //When you first add few buy orders using CreateBuyOrder() method;
        //and then invoke GetAllBuyOrders() method; the returned list should
        //contain all the same buy orders.
        [Fact]
        public async Task GetAllSellOrders_FewOrders()
        {
            SellOrderRequest request1 = new SellOrderRequest()
            {
                StockSymbol = "MSFT",
                StockName = "Microsoft Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 4,
                Price = 100
            };

            SellOrderRequest request2 = new SellOrderRequest()
            {
                StockSymbol = "AAPL",
                StockName = "Apple Stock",
                DateAndTimeOfOrder = DateTime.UtcNow,
                Quantity = 20,
                Price = 5000
            };

            SellOrderResponse response1 = await _stocksService.CreateSellOrder(request1);
            SellOrderResponse response2 = await _stocksService.CreateSellOrder(request2);
            List<SellOrderResponse> expectedList = new List<SellOrderResponse>()
                { response1, response2 };

            ModelValidator.Validate(response1);
            ModelValidator.Validate(response2);

            List<SellOrderResponse> actualList = await _stocksService.GetSellOrders();
            foreach (SellOrderResponse response in expectedList)
            {
                Assert.Contains(response, actualList);
            }
        }

        #endregion
    }
}
