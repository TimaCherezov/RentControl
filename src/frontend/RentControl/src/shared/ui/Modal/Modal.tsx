import { useEffect, useRef, type ReactNode } from "react";
import { cn } from "../../utils/cn";
import { Button } from "../Button";

export type ModalProps = {
  open: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
  className?: string;
};

export function Modal({
  open,
  onClose,
  title,
  children,
  className,
}: ModalProps) {
  const dialogRef = useRef<HTMLDialogElement>(null);

  useEffect(() => {
    const dialog = dialogRef.current;
    if (!dialog) return;

    if (open && !dialog.open) dialog.showModal();
    if (!open && dialog.open) dialog.close();

    return () => {
      if (dialog.open) dialog.close();
    };
  }, [open]);

  return (
    <dialog
      ref={dialogRef}
      aria-label={title}
      onClose={onClose}
      onClick={(event) => {
        if (event.target === dialogRef.current) onClose();
      }}
      className={cn(
        "rounded-lg border border-border bg-bg p-0 text-text shadow-card backdrop:bg-black/50",
        className,
      )}
    >
      <div className="flex items-center justify-between gap-4 border-b border-border p-4">
        <h2 className="text-lg font-medium text-text-h">{title}</h2>
        <Button
          variant="ghost"
          size="sm"
          onClick={onClose}
          aria-label="Закрыть"
        >
          ×
        </Button>
      </div>
      <div className="p-4">{children}</div>
    </dialog>
  );
}