using Moq;

using MoqPuzzles.Models;
using MoqPuzzles.Repositories;
using MoqPuzzles.Services;

namespace MoqPuzzlesTests {
    public sealed class LineItemProcessorTests {
        private const string UNKNOWN_PROCEDURE_CODE = "9999A";
        private readonly decimal DEFAULT_LINE_ITEM_CHARGE = 100.00m;
        private readonly DateTime JULY_01_2024_UTC = new DateTime(2024, 07, 01, 0, 0, 0, DateTimeKind.Utc);

        [Fact]
        public void Processor_returns_null_description_and_null_allowance_value_for_unknown_procedure_code() {
            var lineItemWithUnknownProcedureCode = new LineItem(
                UNKNOWN_PROCEDURE_CODE,
                DEFAULT_LINE_ITEM_CHARGE,
                JULY_01_2024_UTC);
            // Set the mock behavior to strict and configure intentionally null-returning setups.
            //var mockCodeInfoRepository = new Mock<ICodeInfoRepository>();
            var mockCodeInfoRepository = new Mock<ICodeInfoRepository>(MockBehavior.Strict);
            mockCodeInfoRepository
                .Setup(repo => repo.GetDescriptionOfCode(
                    It.Is<string>(procedureCode => procedureCode == UNKNOWN_PROCEDURE_CODE)))
                .Returns((string?)null);
            //var mockMedicareRepository = new Mock<IMedicareRepository>();
            var mockMedicareRepository = new Mock<IMedicareRepository>(MockBehavior.Strict);
            mockMedicareRepository
                .Setup(repo => repo.GetMedicareAllowance(
                    It.Is<string>(procedureCode => procedureCode == UNKNOWN_PROCEDURE_CODE), It.IsAny<DateTime>()))
                .Returns((decimal?)null);
            var processor = new LineItemProcessor(mockCodeInfoRepository.Object, mockMedicareRepository.Object);
            CodeDescriptionAndAllowance codeDescriptionAndAllowance = processor.GetCodeDescriptionAndAllowance(
                lineItemWithUnknownProcedureCode);
            const string? expectedDescription = null;
            decimal? expectedMedicareAllowance = null;
            string? actualDescription = codeDescriptionAndAllowance.Description;
            decimal? actualMedicareAllowance = codeDescriptionAndAllowance.MedicareAllowance;
            Assert.Equal(expectedDescription, actualDescription);
            Assert.Equal(expectedMedicareAllowance, actualMedicareAllowance);
        }

        [Fact]
        public void Processor_returns_null_allowance_value_and_null_description_for_unknown_procedure_code() {
            var lineItemWithUnknownProcedureCode = new LineItem(
                UNKNOWN_PROCEDURE_CODE,
                DEFAULT_LINE_ITEM_CHARGE,
                JULY_01_2024_UTC);
            var mockCodeInfoRepository = new Mock<ICodeInfoRepository>(MockBehavior.Strict);
            // Set up the correct, applicable overload of GetDescriptionOfCode()!
            mockCodeInfoRepository
                .Setup(mockRepo => mockRepo.GetDescriptionOfCode(
                    //It.Is<string>(procedureCode => procedureCode == UNKNOWN_PROCEDURE_CODE),
                    //It.Is<string>(procedureCode => procedureCode == UNKNOWN_PROCEDURE_CODE)))
                    It.Is<string>(procedureCode => procedureCode == UNKNOWN_PROCEDURE_CODE)))
                .Returns((string?)null);
            var mockMedicareRepository = new Mock<IMedicareRepository>(MockBehavior.Strict);
            // Set up the correct, applicable overload of GetMedicareAllowance()!
            mockMedicareRepository
                .Setup(mockRepo => mockRepo.GetMedicareAllowance(
                    //It.Is<string>(procedureCode => procedureCode == UNKNOWN_PROCEDURE_CODE),
                    //It.IsAny<DateTime>(),
                    //It.IsAny<string[]>()))
                    It.Is<string>(procedureCode => procedureCode == UNKNOWN_PROCEDURE_CODE),
                    It.IsAny<DateTime>()))
                //.Returns((string procedureCode, DateTime dateOfService, string[] modifiers) => null);
                .Returns((decimal?)null);
            var processor = new LineItemProcessor(mockCodeInfoRepository.Object, mockMedicareRepository.Object);
            CodeDescriptionAndAllowance codeDescriptionAndAllowance = processor.GetCodeDescriptionAndAllowance(
                lineItemWithUnknownProcedureCode);
            const string? expectedDescription = null;
            decimal? expectedMedicareAllowance = null;
            string? actualDescription = codeDescriptionAndAllowance.Description;
            decimal? actualMedicareAllowance = codeDescriptionAndAllowance.MedicareAllowance;
            Assert.Equal(expectedDescription, actualDescription);
            Assert.Equal(expectedMedicareAllowance, actualMedicareAllowance);
        }
    }
}
