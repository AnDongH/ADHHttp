using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Text;
using ZLogger;

namespace HttpServer {
    public class LogFormatter : IZLoggerFormatter {
        public bool WithLineBreak => true;

        public void FormatLogEntry(IBufferWriter<byte> writer, IZLoggerEntry entry) {
            
            StringBuilder sb = new StringBuilder();

            switch (entry.LogInfo.LogLevel) {
                case LogLevel.Information:
                    sb.Append($"\x1b[38;2;{0};{255};{0}m");
                    break;
                case LogLevel.Warning:
                    sb.Append($"\x1b[38;2;{255};{255};{0}m");
                    break;
                case LogLevel.Error:
                    sb.Append($"\x1b[38;2;{255};{0};{0}m");
                    break;
                default:
                    sb.Append($"\x1b[38;2;{255};{255};{255}m");
                    break;
            }

            sb.Append('[');
            sb.Append(entry.LogInfo.LogLevel);
            sb.Append("]:\n");
            sb.Append($"\x1b[38;2;{255};{255};{255}m");
            sb.Append(entry.LogInfo.Timestamp);
            sb.Append(" - ");
            sb.Append(entry.LogInfo.Category);
            sb.Append('[');
            sb.Append(entry.LogInfo.LineNumber);
            sb.Append("]:\n");
            sb.Append(entry.ToString());

            // 로그 메시지를 바이트 배열로 변환하여 작성
            var utf8Bytes = Encoding.UTF8.GetBytes(sb.ToString());
            writer.Write(utf8Bytes);
        }
    }
}
