"""Build an animated GIF carousel from the Tend screenshots.

Each screen is centered on a uniform canvas tinted with Tend's warm off-white
surface (#FAF8F4), cross-faded into the next, looped infinitely.

Usage:
    python3 -m venv .venv && source .venv/bin/activate
    pip install pillow
    python3 docs/screenshots/build_carousel.py
"""

from pathlib import Path
from PIL import Image

SHOTS_DIR = Path(__file__).resolve().parent
OUT_PATH  = SHOTS_DIR / "carousel.gif"

# Order the user sees in the carousel (mirrors the README's table).
ORDER = ["onboarding", "today", "checkin", "history", "entry-detail"]

# Output canvas. Phone-ish 9:19.5 at 360w keeps the GIF readable but small.
CANVAS_W   = 360
CANVAS_H   = 780
SURFACE    = (250, 248, 244)  # #FAF8F4

# Pacing
FPS         = 15
HOLD_SECS   = 1.8   # how long each screen sits still
FADE_SECS   = 0.35  # crossfade duration between screens
HOLD_FRAMES = int(HOLD_SECS * FPS)
FADE_FRAMES = int(FADE_SECS * FPS)


def load_centered(name: str) -> Image.Image:
    """Load a screenshot, scale to fit inside CANVAS_W x CANVAS_H, center on warm bg."""
    src = Image.open(SHOTS_DIR / f"{name}.png").convert("RGB")
    w, h = src.size
    scale = min(CANVAS_W / w, CANVAS_H / h)
    nw, nh = int(w * scale), int(h * scale)
    resized = src.resize((nw, nh), Image.LANCZOS)
    canvas = Image.new("RGB", (CANVAS_W, CANVAS_H), SURFACE)
    canvas.paste(resized, ((CANVAS_W - nw) // 2, (CANVAS_H - nh) // 2))
    return canvas


def build_frames(images: list[Image.Image]) -> list[Image.Image]:
    """Hold each image, then crossfade into the next. Loops back to the first."""
    frames: list[Image.Image] = []
    n = len(images)
    for i in range(n):
        current = images[i]
        nxt     = images[(i + 1) % n]

        # Hold the current frame.
        frames.extend([current] * HOLD_FRAMES)

        # Crossfade into the next frame (exclude alpha=0 and alpha=1; those are the
        # held frames on either side).
        for f in range(1, FADE_FRAMES):
            alpha = f / FADE_FRAMES
            frames.append(Image.blend(current, nxt, alpha))
    return frames


def main() -> None:
    print(f"Loading {len(ORDER)} screens…")
    images = [load_centered(name) for name in ORDER]

    print(f"Building frames at {FPS}fps "
          f"(hold {HOLD_SECS}s, fade {FADE_SECS}s)…")
    frames = build_frames(images)
    print(f"  {len(frames)} frames total "
          f"({len(frames) / FPS:.1f}s loop)")

    # Pillow palette quantization for sharp, clean GIFs.
    quantized = [
        f.quantize(colors=128, method=Image.MEDIANCUT, dither=Image.Dither.FLOYDSTEINBERG)
        for f in frames
    ]

    duration_ms = int(1000 / FPS)
    quantized[0].save(
        OUT_PATH,
        save_all=True,
        append_images=quantized[1:],
        duration=duration_ms,
        loop=0,                # infinite
        disposal=2,            # replace previous frame (avoids ghosting)
        optimize=True,
    )

    size_kb = OUT_PATH.stat().st_size / 1024
    print(f"Wrote {OUT_PATH} ({size_kb:.0f} KB)")


if __name__ == "__main__":
    main()
