## Infrastructure Notes

### Fargate

Any HTTP services (tagged HTTP) will also have LBs on them. I'm not adding them to the
diagram as it's superfluous and there is a visual indicator.

## Blazor

Blazor was the result of a need to handle configurable authentication in the UI.

## MongoDB

MongoDB ended up as a solution to the problem of needing to update `jsonb` documents
without a race condition.
