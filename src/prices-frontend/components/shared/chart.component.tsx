import c3, { ChartConfiguration } from "c3";
import { memo, useContext, useEffect, useRef } from "react";
import { ModeContext } from "../../contexts/mode.context";
import classes from "./chart.module.scss";

export type ChartProps = {
    config: ChartConfiguration;
    className?: string;
};

export const Chart = memo<ChartProps>(function Chart(props: ChartProps) {
    const { config, className, ...otherProps } = props;
    const { mode } = useContext(ModeContext);
    const classNames = mode === "dark" ? `${classes.chart} ${classes.inverted}` : `${classes.chart}`;

    const chartRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const chart = c3.generate({
            bindto: chartRef.current,
            ...config,
        });

        return function cleanup() {
            chart?.destroy();
        };
    }, [config]);

    return (
        <div className={classNames} style={{ width: "100%" }}>
            <div ref={chartRef} {...otherProps} />
        </div>
    );
});

export default Chart;
