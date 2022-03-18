import { useRef, useState } from "react";

// https://css-tricks.com/dealing-with-stale-props-and-states-in-reacts-functional-components/
export const useAsyncRef = (value) => {
  const ref = useRef(value);
  const [_, forceRender] = useState(false);

  const updateRef = (newState) => {
    ref.current = newState;
    forceRender((s) => !s);
  };

  return [ref, updateRef];
};
