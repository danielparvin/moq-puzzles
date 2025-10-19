using Moq;

using MoqPuzzles.Models;
using MoqPuzzles.Services;

namespace MoqPuzzlesTests {
    public sealed class BillProcessorTests {
        private const string UNKNOWN_PROCEDURE_CODE = "00000";
        private const string SUTURE_PROCEDURE_CODE = "12002";

        [Fact]
        public void Line_item_on_bill_remains_unchanged_when_line_item_has_unknown_procedure_code() {
            //var mockLineItemProcessor = new Mock<ILineItemProcessor>();
            var mockLineItemProcessor = new Mock<ILineItemProcessor>(MockBehavior.Strict);
            mockLineItemProcessor
                .Setup(mockLineItemProcessor => mockLineItemProcessor.GetCodeDescriptionAndAllowance(
                    It.Is<LineItem>(lineItem => lineItem.ProcedureCode == UNKNOWN_PROCEDURE_CODE)))
                .Returns(new CodeDescriptionAndAllowance(UNKNOWN_PROCEDURE_CODE));
            BillProcessor billProcessor = new BillProcessor(mockLineItemProcessor.Object);
            var testBill = new Bill();
            var testLineItem = getTestLineItemWithUnknownProcedureCode();
            testBill.LineItems.Add(testLineItem);
            billProcessor.ProcessBill(testBill);
            // unchangedLineItem was a different class instance, so it was not equal to testLineItem!
            // Compare the adjustments on the two line items instead.
            //var unchangedLineItem = getTestLineItemWithUnknownProcedureCode();
            //Assert.Equal(unchangedLineItem, testLineItem);
            var expectedNumberOfAdjustments = 0;
            var actualNumberOfAdjustments = testBill.LineItems[0].Adjustments.Count;
            Assert.Equal(expectedNumberOfAdjustments, actualNumberOfAdjustments);
        }

        [Fact]
        public void Line_item_on_bill_gets_adjustment_for_suture_procedure_code() {
            //var mockLineItemProcessor = new Mock<ILineItemProcessor>();
            var mockLineItemProcessor = new Mock<ILineItemProcessor>(MockBehavior.Strict);
            mockLineItemProcessor
                // The original code set up the mock to match the line item argument against the return value of 
                // getTestLineItemWithSutureProcedureCode().
                // Match the value of the line item's ProcedureCode instead, using a custom function.
                //.Setup(mockLineItemProcessor =>
                //    mockLineItemProcessor.GetCodeDescriptionAndAllowance(getTestLineItemWithSutureProcedureCode()))
                .Setup(mockLineItemProcessor =>
                    mockLineItemProcessor.GetCodeDescriptionAndAllowance(
                        It.Is<LineItem>(lineItem => lineItem.ProcedureCode == SUTURE_PROCEDURE_CODE)))
                .Returns(new CodeDescriptionAndAllowance(SUTURE_PROCEDURE_CODE) {
                    Description = "Suture Procedure",
                    MedicareAllowance = 50.00m,
                    ProcedureCode = SUTURE_PROCEDURE_CODE
                });
            BillProcessor billProcessor = new BillProcessor(mockLineItemProcessor.Object);
            var testBill = new Bill();
            var testLineItem = getTestLineItemWithSutureProcedureCode();
            testBill.LineItems.Add(testLineItem);
            billProcessor.ProcessBill(testBill);
            int expectedNumberOfAdjustments = 1;
            Assert.Equal(expectedNumberOfAdjustments, testLineItem.Adjustments.Count);
        }

        private LineItem getTestLineItemWithUnknownProcedureCode() {
            var testLineItem = new LineItem(
                UNKNOWN_PROCEDURE_CODE,
                100.00m,
                new DateTime(2023, 01, 01, 00, 00, 00, DateTimeKind.Utc));
            return testLineItem;
        }

        private LineItem getTestLineItemWithSutureProcedureCode() {
            var testLineItem = new LineItem(
                SUTURE_PROCEDURE_CODE,
                100.00m,
                new DateTime(2023, 01, 01, 00, 00, 00, DateTimeKind.Utc));
            return testLineItem;
        }
    }
}
