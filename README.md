# Overview
This repo contains my Prometheus exporter. It allows you to create a multitude of metrics and output them as a simple string that Prometheus can read. It's usage is simple. Create a MetricRegistry, call the appropriate metric methods and set some metrics. It contains a helper for IServiceCollection dependency injection that will add the required classes.

Every interface and method is documented with XML comments to make it easier to use.

# Features
* The base library has a single dependency, the Dependency Injection package.
* Easy to use
* Supported metrics
    * Counter
    * Gauge
    * Histogram
* Extendable
    * Everything exposed uses interfaces.
    * No sealed classes
    * No internal classes
* Permissive license, MIT
* Clean and consistent interfaces
* Small namespace so you don't bring in a bunch of stuff you won't need.
    * The namespace to use is `Vecc.Prometheus`. It currently only has 5 interfaces and 3 classes.
    * The guts of the project is all in `Vecc.Prometheus.Internal`. You will most likely will only need to bring in that namespace if you are extending the project.

# Usage
You can find a simple and easy to follow example in the `/src/Vecc.Prometheus.Example` project.

# Items remaining to work on
* Examples
* Documentation
    * The methods are documented and an example project exists. It would be nice to see more well-rounded documentation around use cases.
* Additional packages
    * Simple extension methods for AspNetCore to expose it to Prometheus
    * Expose an additional http server endpoint to host the scrape
    * Push the scrape to an endpoint
