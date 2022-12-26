A large portion of folks using AGPL use it as a way to strong-arm folks into expensive commercial licenses. I have no such license available.
If you are concerned about whether or not your integrations would be under the scope of the copyleft license here are a few notes.

The viral nature of AGPL (and GPL) do not apply when you are integrating with other systems. Meaning that any systems you create and integrate
from push are not in scope.

If you create a system that interfaces with push direcly, that would be in scope. This is only an issue if your users are external to the company.
Internal systems used by internal clients will not require you to distribute code outside of your company.

If you *do* wish to integrate with push with a publically available system, and not have to distribute the code of that system. There is a workaround.
As long as the system isn't directly connecting to push, or any systems directly connecting to push, the viral nature will not apply. An appropriate
example would be something like creating timed reports using the push API and pushing the results to a database. Since we do not own the copyright
to your data (you do) you can have your publicly available system read from that database without worrying about distributing your source.

Here are some visual representations on what I'm talking about

```
# Case 1: No disclosure
{push}->{your service}

# Case 2: No disclosure
{your service}->{push}
Given: The only users of your service are internal

# Case 3: Requires Disclosure
{your service}->{push}
Given: Users of your service are external

# Case 4: No Disclosure
{your service}->{your database}<-{your other service}->{push}
```
