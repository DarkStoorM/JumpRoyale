namespace JumpRoyale.Utils;

public delegate TReturnType GenericReturnActionHandler<out TReturnType>();

public delegate TReturnType GenericReturnActionHandler<out TReturnType, in TParameterType>(TParameterType param);

public delegate void ActionHandler();

public delegate void GenericActionHandler<in TParameterType>(TParameterType param);

public delegate void GenericActionHandler<in TParameterType, in TValueType>(TParameterType param, TValueType value);
