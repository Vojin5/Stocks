using AutoFixture;
using Entities;
using Entities.DTO;
using Entities.HelperClasses;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests.UnitTests
{
    public class StockServiceTests
    {
        private readonly Mock<IStocksRepository> _stocksRepositoryMock;
        private readonly IStocksRepository _stocksRepository;

        private readonly IStocksService _stocksService;

        private readonly IFixture _fixture;
        public StockServiceTests()
        {
            _stocksRepositoryMock = new Mock<IStocksRepository>();
            _stocksRepository = _stocksRepositoryMock.Object;

            _stocksService = new StocksService(_stocksRepository);

            _fixture = new Fixture();
        }

        #region CreateBuyOrder

        //When you supply BuyOrderRequest as null, it should throw ArgumentNullException.
        [Fact]
        public async Task CreateBuyOrder_NullAsRequest_ToBeArgumentNullException()
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
        public async Task CreateBuyOrder_QuantityAsZero_ToBeArgumentException()
        {
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 0)
                .With(x => x.Price, 100)
                .Create();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateBuyOrder(request);
            });
        }

        //When you supply buyOrderQuantity as 100001
        //(as per the specification, maximum is 100000), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_QuantityOverflow_ToBeArgumentException()
        {
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100001)
                .With(x => x.Price, 100)
                .Create();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateBuyOrder(request);
            });
        }

        //When you supply buyOrderPrice as 0
        //(as per the specification, minimum is 1), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_PriceAsZero_ToBeArgumentException()
        {
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 0)
                .With(x => x.Price, 0)
                .Create();
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateBuyOrder(request);
            });
        }

        //When you supply buyOrderPrice as 10001
        //(as per the specification, maximum is 10000), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_PriceOverflow_ToBeArgumentException()
        {
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 0)
                .With(x => x.Price, 10001)
                .Create();
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateBuyOrder(request);
            });
        }

        //When you supply stock symbol=null (as per the specification,
        //stock symbol can't be null), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_SymbolAsNull_ToBeArgumentException()
        {
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.StockSymbol, null as string)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .Create();
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateBuyOrder(request);
            });
        }

        //When you supply dateAndTimeOfOrder as "1999-12-31" (YYYY-MM-DD)
        //- (as per the specification,
        //it should be equal or newer date than 2000-01-01), it should throw ArgumentException.
        [Fact]
        public async Task CreateBuyOrder_WrongDate_ToBeArgumentException()
        {
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Parse("1999-12-31"))
                .With(x => x.Quantity, 0)
                .With(x => x.Price, 100)
                .Create();
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateBuyOrder(request);
            });
        }

        //If you supply all valid values, it should be successful and
        //return an object of BuyOrderResponse type with auto-generated BuyOrderID (guid).
        [Fact]
        public async Task CreateBuyOrder_ValidRequest_ToBeSucessful()
        {
            BuyOrderRequest request = _fixture.Build<BuyOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .Create();

            BuyOrder expectedBuyOrder = request.ToBuyOrder();
            expectedBuyOrder.BuyOrderID = new Guid();
            BuyOrderResponse expectedBuyOrderResponse = expectedBuyOrder.ToBuyOrderResponse();

            _stocksRepositoryMock.Setup(x => x.CreateBuyOrder(It.IsAny<BuyOrder>()))
                .ReturnsAsync(expectedBuyOrder);

            BuyOrderResponse actualResponse = await _stocksService.CreateBuyOrder(request);
            Assert.NotNull(actualResponse);
            ModelValidator.Validate(actualResponse);
            Assert.Equal(expectedBuyOrderResponse, actualResponse);
        }
        #endregion

        #region CreateSellOrder

        //When you supply SellOrderRequest as null, it should throw ArgumentNullException.
        [Fact]
        public async Task CreateSellOrder_NullAsRequest_ToBeArgumentNullException()
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
        public async Task CreateSellOrder_QuantityAsZero_ToBeArgumentException()
        {
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 0)
                .With(x => x.Price, 100)
                .Create();
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateSellOrder(request);
            });
        }

        //When you supply SellOrderQuantity as 100001
        //(as per the specification, maximum is 100000), it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_QuantityOverflow_ToBeArgumentException()
        {
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100001)
                .With(x => x.Price, 100)
                .Create();
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateSellOrder(request);
            });
        }

        //When you supply SellOrderPrice as 0
        //(as per the specification, minimum is 1), it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_PriceAsZero_ToBeArgumentException()
        {
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 0)
                .Create();
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateSellOrder(request);
            });
        }

        //When you supply SellOrderPrice as 10001
        //(as per the specification, maximum is 10000), it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_PriceOverflow_ToBeArgumentException()
        {
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 10001)
                .Create();
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateSellOrder(request);
            });
        }

        //When you supply stock symbol=null (as per the specification,
        //stock symbol can't be null), it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_SymbolAsNull_ToBeArgumentException()
        {
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.StockSymbol, null as string)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .Create();
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateSellOrder(request);
            });
        }

        //When you supply dateAndTimeOfOrder as "1999-12-31" (YYYY-MM-DD)
        //- (as per the specification,
        //it should be equal or newer date than 2000-01-01), it should throw ArgumentException.
        [Fact]
        public async Task CreateSellOrder_WrongDate_ToBeArgumentException()
        {
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Parse("1999-12-31"))
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .Create();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _stocksService.CreateSellOrder(request);
            });
        }

        //If you supply all valid values, it should be successful and
        //return an object of SellOrderResponse type with auto-generated BuyOrderID (guid).
        [Fact]
        public async Task CreateSellOrder_ValidRequest_ToBeSucessful()
        {
            SellOrderRequest request = _fixture.Build<SellOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 100)
                .With(x => x.Price, 100)
                .Create();
            SellOrder expectedSellOrder = request.ToSellOrder();
            expectedSellOrder.SellOrderID = Guid.NewGuid();
            SellOrderResponse expectedSellOrderResponse = expectedSellOrder.ToSellOrderResponse();

            _stocksRepositoryMock.Setup(x => x.CreateSellOrder(It.IsAny<SellOrder>()))
                .ReturnsAsync(expectedSellOrder);

            SellOrderResponse actualResponse = await _stocksService.CreateSellOrder(request);
            Assert.NotNull(actualResponse);
            ModelValidator.Validate(actualResponse);
            Assert.Equal(expectedSellOrderResponse, actualResponse);
        }

        #endregion

        #region GetAllBuyOrders

        //When you invoke this method, by default, the returned list should be empty.
        [Fact]
        public async Task GetAllBuyOrders_Default()
        {
            _stocksRepositoryMock.Setup(x => x.GetBuyOrders()).ReturnsAsync(new List<BuyOrder>());
            List<BuyOrderResponse> responseList = await _stocksService.GetBuyOrders();
            Assert.Empty(responseList);
        }

        //When you first add few buy orders using CreateBuyOrder() method;
        //and then invoke GetAllBuyOrders() method; the returned list should
        //contain all the same buy orders.
        [Fact]
        public async Task GetAllBuyOrders_FewOrders()
        {
            BuyOrderRequest request1 = _fixture.Build<BuyOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 4)
                .With(x => x.Price, 100)
                .Create();

            BuyOrderRequest request2 = _fixture.Build<BuyOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 20)
                .With(x => x.Price, 1000)
                .Create();

            BuyOrder expectedOrder1 = request1.ToBuyOrder();
            expectedOrder1.BuyOrderID = Guid.NewGuid();
            BuyOrder expectedOrder2 = request2.ToBuyOrder();
            expectedOrder2.BuyOrderID = Guid.NewGuid();

            List<BuyOrder> mockList = new List<BuyOrder>()
            {
                expectedOrder1,expectedOrder2
            };

            _stocksRepositoryMock.Setup(x => x.GetBuyOrders()).ReturnsAsync(mockList);

            List<BuyOrderResponse> expectedList = new List<BuyOrderResponse>()
                { expectedOrder1.ToBuyOrderResponse(),expectedOrder2.ToBuyOrderResponse() };

            List<BuyOrderResponse> actualList = await _stocksService.GetBuyOrders();
            Assert.Equivalent(expectedList, actualList);
        }

        #endregion

        #region GetAllSellOrders

        //When you invoke this method, by default, the returned list should be empty.
        [Fact]
        public async Task GetAllSellOrders_Default()
        {
            _stocksRepositoryMock.Setup(x => x.GetSellOrders()).ReturnsAsync(new List<SellOrder>());
            List<SellOrderResponse> responseList = await _stocksService.GetSellOrders();
            Assert.Empty(responseList);
        }

        //When you first add few buy orders using CreateBuyOrder() method;
        //and then invoke GetAllBuyOrders() method; the returned list should
        //contain all the same buy orders.
        [Fact]
        public async Task GetAllSellOrders_FewOrders()
        {
            SellOrderRequest request1 = _fixture.Build<SellOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 4)
                .With(x => x.Price, 100)
                .Create();

            SellOrderRequest request2 = _fixture.Build<SellOrderRequest>()
                .With(x => x.DateAndTimeOfOrder, DateTime.Now)
                .With(x => x.Quantity, 20)
                .With(x => x.Price, 1000)
                .Create();

            SellOrder expectedOrder1 = request1.ToSellOrder();
            expectedOrder1.SellOrderID = Guid.NewGuid();
            SellOrder expectedOrder2 = request2.ToSellOrder();
            expectedOrder2.SellOrderID = Guid.NewGuid();

            List<SellOrder> mockList = new List<SellOrder>()
            {
                expectedOrder1,expectedOrder2
            };

            _stocksRepositoryMock.Setup(x => x.GetSellOrders()).ReturnsAsync(mockList);

            List<SellOrderResponse> expectedList = new List<SellOrderResponse>()
                { expectedOrder1.ToSellOrderResponse(),expectedOrder2.ToSellOrderResponse() };

            List<SellOrderResponse> actualList = await _stocksService.GetSellOrders();
            Assert.Equivalent(expectedList, actualList);
        }

        #endregion
    }
}
