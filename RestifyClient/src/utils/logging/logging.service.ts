import { Injectable } from '@angular/core';
import { Logger } from 'seq-logging';
import {environment} from '../../environments/environment';

type SeqLogLevel = 'Verbose' | 'Debug' | 'Information' | 'Warning' | 'Error' | 'Fatal';

@Injectable({
  providedIn: 'root',
})
export class LoggingService {
  private readonly seq: Logger;
  constructor() {
      this.seq = new Logger({ serverUrl: environment.SEQ_URL, onError: (e) => console.error(e) });
  }

    info(message: string, properties: object = {}) {
        this.emitLog('Information', message, properties);
    }

    warn(message: string, properties: object = {}) {
        this.emitLog('Warning', message, properties);
    }

    error(message: string, error?: any, properties: object = {}) {
        // Automatically extract error details if provided
        const errorProps = error instanceof Error
            ? { errorMessage: error.message, stack: error.stack }
            : { error };

        this.emitLog('Error', message, { ...properties, ...errorProps });
    }

    debug(message: string, properties: object = {}) {
        this.emitLog('Debug', message, properties);
    }

    private emitLog(level: SeqLogLevel, message: string, properties: object) {
        this.seq.emit({
            timestamp: new Date(),
            level: level,
            messageTemplate: message,
            properties: {
                ...properties,
                App: 'RestifyClient',
                Environment: environment.production ? 'Production' : 'Development'
            }
        });

        if (!environment.production) {
            const color = level === 'Error' ? 'red' : level === 'Warning' ? 'orange' : 'skyblue';
            console.log(`%c[${level}] ${message}`, `color: ${color}`, properties);
        }
    }
}
