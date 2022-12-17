import styled from "@emotion/styled";
import { ToggleButton, ToggleButtonGroup } from "@mui/material";
import { memo, useEffect, useState } from "react";
import { ChangeSpan } from "../../gql/codegen/graphql";

type SpanSelectorProps = {
    span: ChangeSpan;
    busy: boolean;
    onSpanChange: (e: React.MouseEvent<HTMLElement>, value: ChangeSpan) => void;
};

type ToggleButtonGroupProps = {
    busy: string | undefined;
};

const $ToggleButtonGroup = styled(ToggleButtonGroup)<ToggleButtonGroupProps>(
    ({ busy }) => `
  .MuiToggleButton-root {
    border: 0;
    border-radius: 0;
  }

  .MuiToggleButton-root.Mui-selected {
    background-color: unset;
    cursor: ${busy};
  }
`
);

export const SpanSelector = memo<SpanSelectorProps>(function SpanSelector(props: SpanSelectorProps) {
    const { span, busy, onSpanChange } = props;
    const [activity, setActivity] = useState<string | undefined>(undefined);

    useEffect(() => {
        if (busy) {
            const timeout = setTimeout(() => {
                setActivity("progress");
            }, 500);

            return () => {
                clearTimeout(timeout);
                setActivity(undefined);
            };
        }
    }, [busy]);

    return (
        <$ToggleButtonGroup
            color="primary"
            value={span}
            exclusive
            busy={activity}
            onChange={(e, value) => {
                if (value) {
                    onSpanChange(e, value);
                }
            }}
        >
            <ToggleButton value={ChangeSpan.Hour}>1H</ToggleButton>
            <ToggleButton value={ChangeSpan.Day}>1D</ToggleButton>
            <ToggleButton value={ChangeSpan.Week}>1W</ToggleButton>
            <ToggleButton value={ChangeSpan.Month}>1M</ToggleButton>
            <ToggleButton value={ChangeSpan.Year}>1Y</ToggleButton>
            <ToggleButton value={ChangeSpan.All}>ALL</ToggleButton>
        </$ToggleButtonGroup>
    );
});
