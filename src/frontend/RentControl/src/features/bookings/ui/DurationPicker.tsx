import { Button, Input } from "../../../shared/ui";
import {
  formatDurationShort,
  formatMaxDuration,
} from "../../../shared/utils/format";

const HOUR = 3_600;

type DurationPickerProps = {
  valueSeconds: number;
  maxSeconds: number;
  onChange: (seconds: number) => void;
  disabled?: boolean;
};

export function DurationPicker({
  valueSeconds,
  maxSeconds,
  onChange,
  disabled = false,
}: DurationPickerProps) {
  const maxHours = Math.floor(Math.max(0, maxSeconds) / HOUR);
  const totalHours = Math.min(
    Math.floor(Math.max(0, valueSeconds) / HOUR),
    maxHours,
  );
  const days = Math.floor(totalHours / 24);
  const hours = totalHours % 24;

  if (maxSeconds <= 0) {
    return (
      <div className="rounded-md border border-dashed border-border p-4 text-sm text-text">
        Нет доступных единиц на выбранную дату.
      </div>
    );
  }

  if (maxHours === 0) {
    return (
      <div className="rounded-md border border-dashed border-border p-4 text-sm text-text">
        Доступно меньше часа. Выберите другое время начала.
      </div>
    );
  }

  const setTotalHours = (next: number) => {
    const clamped = Math.min(Math.max(0, Math.floor(next)), maxHours);
    onChange(clamped * HOUR);
  };

  const rows = [
    {
      key: "days",
      label: "Дни",
      value: days,
      decrementDisabled: disabled || days === 0,
      incrementDisabled: disabled || totalHours + 24 > maxHours,
      set: (next: number) => setTotalHours(next * 24 + hours),
      step: (value: number) => value + 1,
      input: (value: number) => setTotalHours(value * 24 + hours),
    },
    {
      key: "hours",
      label: "Часы",
      value: hours,
      decrementDisabled: disabled || hours === 0,
      incrementDisabled: disabled || totalHours + 1 > maxHours,
      set: (next: number) => setTotalHours(days * 24 + next),
      step: (value: number) => value + 1,
      input: (value: number) => setTotalHours(days * 24 + value),
    },
  ];

  return (
    <div className="flex flex-col gap-3">
      <div className="flex flex-col gap-2">
        {rows.map((row) => (
          <div
            key={row.key}
            className="grid grid-cols-[48px_1fr_auto_auto] items-center gap-2"
          >
            <span className="text-sm text-text">{row.label}</span>
            <Input
              type="number"
              inputMode="numeric"
              min={0}
              value={row.value}
              disabled={disabled}
              className="no-spinner text-center"
              onChange={(event) => {
                const parsed = Number.parseInt(event.target.value, 10);
                row.input(Number.isNaN(parsed) ? 0 : parsed);
              }}
            />
            <Button
              variant="secondary"
              size="sm"
              disabled={row.decrementDisabled}
              onClick={() => row.set(row.value - 1)}
            >
              −
            </Button>
            <Button
              variant="secondary"
              size="sm"
              disabled={row.incrementDisabled}
              onClick={() => row.set(row.step(row.value))}
            >
              +
            </Button>
          </div>
        ))}
      </div>
      <div className="flex items-center justify-between text-xs text-text">
        <span aria-live="polite">
          Итого:{" "}
          <span className="font-medium text-text-h">
            {formatDurationShort(valueSeconds)}
          </span>
        </span>
        <span>Макс.: {formatMaxDuration(maxSeconds)}</span>
      </div>
    </div>
  );
}