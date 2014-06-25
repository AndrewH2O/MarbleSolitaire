WARNING some tests are long running these conditionally compiled by adding 
pre compile flag LONGRUNNINGON and TestEnumerateSolnsFromCompleteFile() generates a DAT file 
that other tests use. 

Also in test explorer some have the trait 'LongerRuntime' by selecting Traits from the drop down filter.

(Total runtime using 1 core 3.4ghz approx 8min. Use 8gb ram.)

TO RUN THESE TESTS add conditional compilation symbol (by right clicking on test project
and clicking under the build tab) to: LONGRUNNINGON


Originally used pre  defines on file by file basis: 

TestSolverEnum - all
TestEnumSolutions - all
TestEnumSolverIO -2 test
TestSolver2 - 2 test


81 Tests
with conditional compile flag of LONGRUNNINGON
97 Tests


