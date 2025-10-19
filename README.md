# Moq Puzzles
## Description
How well do you know the [Moq](https://github.com/devlooped/moq) unit testing library? Are you familiar with some of the most common gotchas?

Something's wrong with each of the unit tests in the MoqPuzzleTests project. Even the test that's passing is wrong! Can you diagnose and resolve the problems?

## Solutions
### BillProcessorTests
#### Line_item_on_bill_remains_unchanged_when_line_item_has_unknown_procedure_code
When you run this test on the main branch, the test fails with the message, "Values differ." But the expected values and actual values read exactly the same!

Expected: LineItem \{ Adjustments = [], Charge = 100.00, DateOfService = 2023-01-01T00:00:00.0000000Z, ProcedureCode = "00000" \}

Actual:   LineItem \{ Adjustments = [], Charge = 100.00, DateOfService = 2023-01-01T00:00:00.0000000Z, ProcedureCode = "00000" \}

Even though these `ToString()` representations are identical, the two `LineItem` instances are not equal because they have distinct reference values. Even if the values of the properties in the `testLineItem` and `unchangedLineItem` `LineItem` objects were exactly the same, the `Assert.Equal` method would still throw an `EqualException`, since the `Assert.Equal` method compares the distinct references of the two `LineItem` instances by default. The solution, then, is to compare the number of adjustments on the `testLineItem` and `unchangedLineItem` instances. (Alternatively, you could implement custom `Equals` and `GetHashCode` overrides on the `LineItem` model.)

#### Line_item_on_bill_gets_adjustment_for_suture_procedure_code
Just like the other test in `BillProcessorTests`, this test fails because the test tries to compare two similar instances of `LineItem` with distinct references. But the failed comparison in this test occurs within the argument-matching logic of the mock. The test sets up the mock line item processor's `GetCodeDescriptionAndAllowance()` method to match on the return value of the `getTestLineItemWithSutureProcedureCode()` method. The return value is a different instance of `LineItem` than the `testLineItem` that the test processes, so the mocked `GetCodeDescriptionAndAllowance` method returns `null` and the test fails. The solution is to match the value of the line item's `ProcedureCode` instead, using a custom predicate function within `It.Is<LineItem>()`.

### LineItemProcessorTests
#### Processor_returns_null_description_and_null_allowance_value_for_unknown_procedure_code
This test passes without actually testing anything. It's a false positive! Moq's default behavior for methods of a mocked interface is to return default values. In this case, the mocked `ICodeInfoRepository` and `IMedicareRepository` both return `null` when the `LineItemProcessor` calls `GetDescriptionOfCode` and `GetMedicareAllowance`. The solution is to specify strict mock behavior when instantiating the mocks and to specifically configure the mocked methods to return `null` description and allowance values for an unknown procedure code.

#### Processor_returns_null_allowance_value_and_null_description_for_unknown_procedure_code
This fails because the test sets up the wrong overloads of the mocked methods. The message, "All invocations on the mock must have a corresponding setup," is a telltale sign that the method under test is calling a mocked method that lacks a relevant setup. To fix this, simply set up the mocks using the correct, applicable overloads of `ICodeInfoRepository.GetDescriptionOfCode()` and `IMedicareRepository.GetMedicareAllowance()`.