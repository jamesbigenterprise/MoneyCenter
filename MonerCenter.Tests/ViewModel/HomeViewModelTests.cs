using MoneyCenter.Model;
using MoneyCenter.Schema;
using Moq;
using FluentAssertions;

namespace MoneyCenter.ViewModel.Tests
{

    public class HomeViewModelTests
    {
        private MainViewModel _viewmodel;
        private Mock<IModel> _modelMock;

        public HomeViewModelTests()
        {
            
            _modelMock = new Mock<IModel>();
            //_viewmodel = new MainViewModel(_modelMock.Object);

        }

        [Fact]
        public async Task PopulateExpenses_Should_Populate_Expenses_List()
        {
            // Arrange
            var date1 = new DateTime(2023, 6, 15);
            var date2 = new DateTime(2023, 6, 14);
            var currentEntries = new List<SingleEntryDataModel>
        {

            new SingleEntryDataModel
            {
                Id = 1,
                Category = "Groceries",
                Details = "Bought groceries",
                Amount = "50.00m",
                Date = date1
            },
            new SingleEntryDataModel
            {
                Id = 2,
                Category = "Utilities",
                Details = "Paid electricity bill",
                Amount = "75.00m",
                Date = date2
            }
        };

            _modelMock.Setup(repo => repo.GetAllEntries()).ReturnsAsync(currentEntries);

            //// Act
            //await _viewmodel.populateExpenses();

            //// Assert
            //_viewmodel.Expenses.Should().HaveCount(2);
            //_viewmodel.Expenses.Should().ContainSingle(e => e.Id == 1 && e.Category == "Groceries" && e.Amount == "50.00m" && e.Date == date1.ToShortDateString() && e.Paragraph == "Bought groceries");
            //_viewmodel.Expenses.Should().ContainSingle(e => e.Id == 2 && e.Category == "Utilities" && e.Amount == "75.00m" && e.Date == date2.ToShortDateString() && e.Paragraph == "Paid electricity bill");
            _modelMock.Verify(repo => repo.GetAllEntries(), Times.Once);
        }

    }
}