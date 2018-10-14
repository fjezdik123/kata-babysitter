#
1)	Project is created with .Net Framework 4.7.2
2)	All tests can be run in Visual Studio -> Test tab -> Windows -> Test Explorer -> by clicking on top link ‘Run All’
3)	Calculations are performed asynchronously
4)	Dropdowns are cached - used Ninject IocContainer to make the cachemanager testable
5)	Implemented validation for bad input
6)	Responsive design (Mobile friendly) implemented
7)	Following logic implemented in the solution:
    The babysitter:
    •	starts no earlier than 5:00PM
    •	leaves no later than 4:00AM
    •	gets paid $12/hour from start-time to bedtime
    •	gets paid $8/hour from bedtime to midnight
    •	gets paid $16/hour from midnight to end of job
    •	gets paid for full hours (no fractional hours)
8)	I have used the following mapping for the solution and divider was midnight:
      Time  Hours
      5PM     0
      6PM	    1
      7PM     2
      8PM     3
      9PM     4
      10PM    5
      11PM    6
      12AM    7
      1AM     8
      2AM     9
      3AM     10
      4AM     11
