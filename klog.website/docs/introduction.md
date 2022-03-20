---
sidebar_position: 1
---

# Introduction

## Welcome

Welcome! And thank you for taking the time to look into and consider KLog!

The goal of KLog is to be a simple, developer focused logging system that's easy
to setup and use. It's open source and can be found on
[GitHub](https://github.com/KTech-Industries/KLog).

## Purpose

KLog came about after spending some time looking into various logging systems
such as Splunk, LogDNA, Sumo Logic, and others. I just wanted something simple
to use locally that was easy to install and could monitor some basic
applications I was building / running.

Essentially, you create an API key that represents a particular application,
and then send logs via an API. Currently, from within the KLog web portal, you
can view a feed of the incomming logs. Eventually there will be alerting, search
and dashboard features as well.

## Features

- Sign in and create local accounts
- Create / delete Api keys
- Send logs with jwt or api key authentication
- View and search a live feed of incoming logs

## Project Structure

### KLog.Api

The KLog.Api project is the core piece of KLog. It contains all of the logic and
handles user creation, log ingestion, and real-time updates via SignalR. It's
built using C# and .Net 6.

### KLog.DataModel

KLog.DataModel consists of the data models used within KLog. This includes
database and table definitions, as well as DTOs used to transfer data between
different parts of the application.

### KLog.DataModel.Migrations

KLog.DataModel.Migrations contains definitions of the migrations between various
versions of KLog. Each of the migrations is run in-process during the startup
of the Klog API.

### klog.web-portal

klog.web-portal is the web interface used to interact with KLog. It's built
with Javascript and React.

### klog.website

klog.website contains the docusaurus project that's running this site
