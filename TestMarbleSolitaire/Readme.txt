WARNING some tests are long running these conditionally compiled by adding 
pre compile flag LONGRUNNINGON

TO RUN THESE TESTS add/remove conditional compilation symbol (by right clicking on test project
and clicking under the build tab) : LONGRUNNINGON

Also in test explorer some have the trait 'LongerRuntime' - select Traits from the drop down filter.
Tests create data on demand and are isolated as a result. Once the test data is created in the 
current appdomain path the files are reused.

(Total runtime using 1 core 3.4ghz approx 12min. Use 8gb ram.)




Originally used the #define directive on a file by file basis: 

TestSolverEnum - all
TestEnumSolutions - all
TestEnumSolverIO -2 test
TestSolver2 - 2 test


81 Tests
with conditional compile flag of LONGRUNNINGON
97 Tests


