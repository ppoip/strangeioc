/// The Context is where all the magic really happens.
/// ===========
/// Other copying the constructors, all you really need to do when you create
/// your context is override Context or one of its subclasses, then set up
/// your list of mappings.
/// 
/// In an MVCSContext, like the one we're using, there are three types of
/// available mappings:
/// 1. Dependency Injection - Bind your dependencies to injectionBinder.
/// 2. View/Mediator Binding - Bind MonoBehaviours on your GameObjects to Mediators that speak to the rest of the app
/// 3. Event Binding - Bind Events to any/all of the following:
/// 		- Event/Method Binding -	Firing the event will trigger the method(s).
/// 		- Event/Command Binding -	Firing the event will instantiate the Command(s) and run its Execute() method.
/// 		- Event/Sequence Binding -	Firing the event will instantiate a Command(s), run its Execute() method, and,
/// 									unless the sequence is interrupted, fire each subsequent Command until the
/// 									sequence is complete.

using System;
using UnityEngine;
using babel.extensions.context.api;
using babel.extensions.context.impl;
using babel.extensions.dispatcher.eventdispatcher.api;
using babel.extensions.dispatcher.eventdispatcher.impl;

namespace babel.examples.myfirstproject
{
	public class ExampleContext : MVCSContext
	{
		
		
		public ExampleContext () : base()
		{
		}
		
		public ExampleContext (MonoBehaviour view, bool autoStartup) : base(view, autoStartup)
		{
		}
		
		protected override void mapBindings()
		{
			//Injection binding.
			//Note that we ALREADY have an injection of IEventDispatcher in MVCSContext,
			//which provides us with a global event bus.
			//
			//So how can Babel tell the difference?
			//The global injection uses a 'named singleton' binding to mark that injection out
			//as different from this one. This binding will generate a new instance of EventDispatcher
			//Whenever we want one. It's being used in ExampleView to facilitate local communication
			//between that View and ExampleMediator.
			injectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>();
			//Map a mock model and a mock service, both as Singletons
			injectionBinder.Bind<IExampleModel>().To<ExampleModel>().AsSingleton();
			injectionBinder.Bind<IExampleService>().To<ExampleService>().AsSingleton();

			//View/Mediator binding
			//This Binding instantiates a new ExampleMediator whenever as ExampleView
			//Fires its Awake method. The Mediator communicates to/from the View
			//and to/from the App. This keeps dependencies between the view and the app
			//separated.
			mediationBinder.Bind<ExampleView>().To<ExampleMediator>();
			
			//Event/Command binding
			//The START event is fired as soon as mappings are complete
			commandBinder.Bind(ContextEvent.START).To<StartCommand>();
			commandBinder.Bind(ExampleEvent.REQUEST_WEB_SERVICE).To<CallWebServiceCommand>();

		}
	}
}

