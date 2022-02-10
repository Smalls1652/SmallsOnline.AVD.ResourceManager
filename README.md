# Azure Virtual Desktop Resource Manager

**Current build status**

[![Build](https://github.com/Smalls1652/SmallsOnline.AVD.ResourceManager/actions/workflows/build.yml/badge.svg?event=push&branch=main)](https://github.com/Smalls1652/SmallsOnline.AVD.ResourceManager/actions/workflows/build.yml)

Shutdown/deallocate Azure Virtual Desktop session hosts, when they're no longer being actively used, to save money on Azure costs.

## 🤔 The motive

With Azure Virtual Desktop, you can create hostpools that are either multi-session capable, which allows multiple users to connect to a single VM at one time that's load-balanced across a pool of VMs, or are "personally assigned" to an individual user, which allows a single user to have a VM assigned to them and no other user. A lot of times, the multi-session hostpools are the goto for standing up remote desktops; however, there are times when personally assigned VMs need to be used.

There's a huge catch to running personally assigned VMs though: **Those VMs will incur costs if they're not deallocated**. Microsoft has had the ["Start VM on connect"](https://techcommunity.microsoft.com/t5/azure-virtual-desktop-blog/start-vm-on-connect-enters-ga/ba-p/2595282) feature, which has been generally available since July 2021, as an option for _starting_ the VM when it's in a deallocated state, but how do you deallocate the VM without interrupting the user experience?

With this solution, you can have those VMs deallocate when they are not in active use to save money. This will also incur costs, but, in my current usage, it's only costing less than $2/month (USD) to run; however, your mileage may vary, but it will pale in comparison to the costs of running a VM for 24 hours per day.

## 🪄 How it works

Every 20 minutes it triggers an evaluation on each hostpool, that you configure it to check, and each VM in that hostpool. If a VM hasn't had an active session twice (Equal to 40 minutes of no sessions), it will deallocate the VM. In the future this will be configurable to be higher.

On the backend, it's using these following Azure services:

* **Azure Functions**
  * For running the evaluation checks.
* **Azure CosmosDB**
  * For storing data about the hostpools and their VMs.
* **Azure User-assigned Managed Identity**
  * For authenticating with the Azure resources.

## 🏗️ Setting up

For info on how to deploy this into your own Azure subscription, [check out the docs](./docs/README.md)!

## 🧑‍💻 Building from source

You will need the following tools to build from source:

* [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
* [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local)

## 🤝 License

This project is licensed under the [MIT License](./LICENSE).
