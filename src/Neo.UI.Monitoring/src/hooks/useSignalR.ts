'use client';

import { useState, useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';

interface UseSignalROptions {
  hubUrl?: string;
  autoConnect?: boolean;
}

export function useSignalR(options: UseSignalROptions = {}) {
  const { hubUrl = '/hubs/monitoring', autoConnect = true } = options;
  const [isConnected, setIsConnected] = useState(false);
  const [lastUpdate, setLastUpdate] = useState<Date | null>(null);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    if (!autoConnect) return;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    connectionRef.current = connection;

    // Connection event handlers
    connection.onreconnecting(() => {
      console.log('SignalR: Reconnecting...');
      setIsConnected(false);
    });

    connection.onreconnected(() => {
      console.log('SignalR: Reconnected');
      setIsConnected(true);
    });

    connection.onclose(() => {
      console.log('SignalR: Connection closed');
      setIsConnected(false);
    });

    // Message handlers
    connection.on('ReceiveMetricUpdate', (metric) => {
      console.log('Metric update:', metric);
      setLastUpdate(new Date());
    });

    connection.on('ReceiveLogEntry', (logEntry) => {
      console.log('Log entry:', logEntry);
      setLastUpdate(new Date());
    });

    connection.on('ReceiveTraceUpdate', (trace) => {
      console.log('Trace update:', trace);
      setLastUpdate(new Date());
    });

    // Start connection
    const startConnection = async () => {
      try {
        await connection.start();
        console.log('SignalR: Connected');
        setIsConnected(true);
      } catch (err) {
        console.error('SignalR: Connection error', err);
        setIsConnected(false);
        
        // Retry after 5 seconds
        setTimeout(startConnection, 5000);
      }
    };

    startConnection();

    // Cleanup
    return () => {
      if (connection.state === signalR.HubConnectionState.Connected) {
        connection.stop();
      }
    };
  }, [hubUrl, autoConnect]);

  const sendMessage = async (methodName: string, ...args: any[]) => {
    if (connectionRef.current?.state === signalR.HubConnectionState.Connected) {
      try {
        await connectionRef.current.invoke(methodName, ...args);
      } catch (err) {
        console.error(`SignalR: Error invoking ${methodName}`, err);
      }
    }
  };

  return {
    isConnected,
    lastUpdate,
    sendMessage,
    connection: connectionRef.current,
  };
}

